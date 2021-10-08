using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CadastroPessoaFisica
{
    internal class DataHelper
    {
        /// <summary>
        /// Cadeia de conexão com o Banco
        /// </summary>
        private static string ConnectionString;
        
        /// <summary>
        /// Define a cadeia de conexão com o Banco
        /// </summary>
        /// <param name="connectionString">Cadeia de conexão com o banco de dados</param>
        static void SetConnectionString(string connectionString)
            => connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        
        /// <summary>
        /// Verifica se a partir da cadeia de conexão foi possível conectar ao banco de dados
        /// </summary>
        /// <exception cref="InvalidOperationException">Lança essa exceção caso a cadeia de conexão não seja válida</exception>
        protected void ValidateConnectionString(){

            SqlConnection con = null;
            InvalidOperationException exception = new InvalidOperationException(message: "Cadeia de conexão não é válida.");
            try
            {
                con = BuildConnection();
            }
            catch (Exception)
            {
                throw exception;
            }
            finally
            {
                con.Close();
            }
        }
        /// <summary>
        /// Realiza a instância da conexão para o banco de dados já aberta.
        /// </summary>
        /// <returns><see cref="SqlConnection"/> Conexão com o bando de dados já aberta</returns>
        protected SqlConnection BuildConnection()
        {
            SqlConnection conn = new SqlConnection(connectionString: ConnectionString);
            if (conn.State != System.Data.ConnectionState.Closed)
                conn.Close();
            conn.Open();

            return conn;
        }
        /// <summary>
        /// Executa a expressão de consulta junto ao banco de dados.
        /// <para>
        /// Expressões que não são permitidas: 
        /// <list type="bullet">
        /// <item name="Insert">Insert</item>
        /// <item name="Update">Update</item>
        /// <item name="Delete">Delete</item>
        /// <item name="Create">Create</item>
        /// <item name="Drop">Drop</item>
        /// <item name="Alter">Alter</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="expression">Expressão de consulta da ser executada</param>
        /// <returns></returns>
        protected DataTable ExecuteQueryStatement(string expression, List<SqlParameter> parameters = null)
        {
            DataTable response = null;
            //Valida a expressão
            if (IsExpressionValid(expression: expression, allowedOperation: null) == false)
                throw new InvalidOperationException(message: "Expressão inválida. É permitido apenas operação de consulta.");
            #region Executa a expressão junto ao banco
            using (SqlConnection connection = BuildConnection())
            {
                try
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(selectCommandText: expression, selectConnection: connection))
                    {
                        if (parameters != null)
                            parameters.ForEach(parameter => adapter.SelectCommand.Parameters.Add(value: parameter));
                        response = new DataTable();
                        adapter.Fill(dataTable: response);
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
            #endregion
            return response;
        }
        /// <summary>
        /// Verifica se a expressão está valida para o tipo de comando permitido | que será executado
        /// </summary>
        /// <param name="expression">Expresão a ser verificada</param>
        /// <param name="allowedOperation">Operação que deve ser considerada como permitida:
        /// <para>
        /// <list type="bullet">
        /// <item name="Insert">Insert |</item>
        /// <item name="Update">Update |</item>
        /// <item name="Delete">Delete |</item>
        /// <item name="Create">Create |</item>
        /// <item name="Drop">Drop |</item>
        /// <item name="Alter">Alter </item>
        /// </list>
        /// </para></param>
        /// <returns><see cref="bool"/> Indicando se a expressão é válida</returns>
        protected bool IsExpressionValid(string expression, string allowedOperation)
        {
            ///Crio a lista com as operações não permitidas | SELECT sempre será permitido inclusive para os casos de subquery
            List<string> invalidOperations = new List<string> {"INSERT", "UPDATE", "DELETE", "CREATE", "DROP", "ALTER" };
            //Removo a operação que deve ser considerada como permitida
            invalidOperations.Remove(item: (allowedOperation ?? "").ToUpper().Trim());
            //Coloco a expressão em maiusculo
            expression = (expression ?? "").ToUpper();
            //Retorno a verificação para saber se dentro da expressão possui algum comando não permitido.
            return expression.Split(separator: new char[] { ' ' }).ToList().Select(item => item.Trim()).Where(item => invalidOperations.Contains(item)).Count() == 0;
        }
    }
}
