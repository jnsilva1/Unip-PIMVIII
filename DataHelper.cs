using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CadastroPessoaFisica
{
    /// <summary>
    /// Facilitador de execução de comando junto ao banco.
    /// </summary>
    /// <remarks>Necessário que seja configurada a cadeia de conexão com o banco antes de realizar qualquer operação. <see cref="SetConnectionString(string)"/></remarks>
    internal class DataHelper
    {
        /// <summary>
        /// Cadeia de conexão com o Banco
        /// </summary>
        private static string ConnectionString;
        protected DataHelper()
        {
            ValidateConnectionString();
        }        
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
        /// <param name="parameters">Parâmetros a serem introduzidos no comando a ser executado.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Quando a expressão tiver comandos não permitidos</exception>
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
        /// Executa a expressão de inserção junto ao banco de dados.
        /// <para>
        /// Expressões que não são permitidas: 
        /// <list type="bullet">
        /// <item name="Update">Update</item>
        /// <item name="Delete">Delete</item>
        /// <item name="Create">Create</item>
        /// <item name="Drop">Drop</item>
        /// <item name="Alter">Alter</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="expression">Expressão de inserção da ser executada</param>
        /// <param name="parameters">Parâmetros a serem introduzidos no comando a ser executado.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Quando a expressão tiver comandos não permitidos</exception>
        protected bool ExecuteInsertStatement(string expression, List<SqlParameter> parameters = null)
        {
            bool response = false;
            if (IsExpressionValid(expression: expression, allowedOperation: "INSERT") == false)
                throw new InvalidOperationException(message: "Expressão inválida. É permitido apenas operação de inserção.");
            #region Executa a expressão junto ao banco
            using (SqlConnection connection = BuildConnection())
            {
                try
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter())
                    {
                        adapter.InsertCommand = new SqlCommand(cmdText: expression, connection: connection);
                        if (parameters != null)
                            parameters.ForEach(parameter => adapter.InsertCommand.Parameters.Add(value: parameter));
                        response = adapter.InsertCommand.ExecuteNonQuery() > 0;
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
        /// Executa a expressão de atualização junto ao banco de dados.
        /// <para>
        /// Expressões que não são permitidas: 
        /// <list type="bullet">
        /// <item name="Insert">Insert</item>
        /// <item name="Delete">Delete</item>
        /// <item name="Create">Create</item>
        /// <item name="Drop">Drop</item>
        /// <item name="Alter">Alter</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="expression">Expressão de alteração da ser executada</param>
        /// <param name="parameters">Parâmetros a serem introduzidos no comando a ser executado.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Quando a expressão tiver comandos não permitidos</exception>
        protected bool ExecuteUpdateStatement(string expression, List<SqlParameter> parameters = null)
        {
            bool response = false;
            if (IsExpressionValid(expression: expression, allowedOperation: "UPDATE") == false)
                throw new InvalidOperationException(message: "Expressão inválida. É permitido apenas operação de atualização.");
            #region Executa a expressão junto ao banco
            using (SqlConnection connection = BuildConnection())
            {
                try
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter())
                    {
                        adapter.UpdateCommand = new SqlCommand(cmdText: expression, connection: connection);
                        if (parameters != null)
                            parameters.ForEach(parameter => adapter.UpdateCommand.Parameters.Add(value: parameter));
                        response = adapter.UpdateCommand.ExecuteNonQuery() > 0;
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
        /// Executa a expressão de exclusão junto ao banco de dados.
        /// <para>
        /// Expressões que não são permitidas: 
        /// <list type="bullet">
        /// <item name="Insert">Insert</item>
        /// <item name="Update">Update</item>
        /// <item name="Create">Create</item>
        /// <item name="Drop">Drop</item>
        /// <item name="Alter">Alter</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="expression">Expressão de exclusão da ser executada</param>
        /// <param name="parameters">Parâmetros a serem introduzidos no comando a ser executado.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Quando a expressão tiver comandos não permitidos</exception>
        protected bool ExecuteDeleteStatement(string expression, List<SqlParameter> parameters = null)
        {
            bool response = false;
            if (IsExpressionValid(expression: expression, allowedOperation: "DELETE") == false)
                throw new InvalidOperationException(message: "Expressão inválida. É permitido apenas operação de exclusão.");
            #region Executa a expressão junto ao banco
            using (SqlConnection connection = BuildConnection())
            {
                try
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter())
                    {
                        adapter.DeleteCommand = new SqlCommand(cmdText: expression, connection: connection);
                        if (parameters != null)
                            parameters.ForEach(parameter => adapter.DeleteCommand.Parameters.Add(value: parameter));
                        response = adapter.DeleteCommand.ExecuteNonQuery() > 0;
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
        /// Executa a expressão junto ao banco de dados.
        /// <para>
        /// Expressões que não são permitidas: 
        /// <list type="bullet">
        /// <item name="Insert">Insert</item>
        /// <item name="Update">Update</item>
        /// <item name="Delete">Delete</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="expression">Expressão a ser executada</param>
        /// <param name="parameters">Parâmetros a serem introduzidos no comando a ser executado.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Quando a expressão tiver comandos não permitidos</exception>
        protected int ExecuteSQLStatement(string expression, List<SqlParameter> parameters = null)
        {
            int response = 0;
            if (IsExpressionValid(expression: expression, allowedOperation: "DROP ALTER CREATE") == false)
                throw new InvalidOperationException(message: "Expressão inválida. É permitido apenas operações: Create, Drop e Alter [Table, Column].");
            #region Executa a expressão junto ao banco
            using (SqlConnection connection = BuildConnection())
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(cmdText: expression, connection: connection))
                    {
                        if (parameters != null)
                            parameters.ForEach(parameter => command.Parameters.Add(value: parameter));
                        response = command.ExecuteNonQuery();
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
            (allowedOperation ?? "").ToUpper().Split(separator: new char[] { ' ' }).ToList().ForEach(allowed => invalidOperations.Remove(item: allowed));
            //Coloco a expressão em maiusculo
            expression = (expression ?? "").ToUpper();
            //Retorno a verificação para saber se dentro da expressão possui algum comando não permitido.
            return expression.Split(separator: new char[] { ' ' }).ToList().Select(item => item.Trim()).Where(item => invalidOperations.Contains(item)).Count() == 0;
        }

        #region Métodos estáticos a serem utilizados dentro do pacote
        /// <summary>
        /// Executa o comando de consulta junto ao banco de dados.
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
        /// <param name="query">Comando de consulta da ser executada</param>
        /// <param name="parameters">Parâmetros a serem introduzidos no comando a ser executado.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Quando a expressão tiver comandos não permitidos</exception>
        public static DataTable ExecuteQuery(string query, List<SqlParameter> parameters = null)
            => new DataHelper().ExecuteQueryStatement(expression: query, parameters);
        /// <summary>
        /// Executa o comando de inserção junto ao banco de dados.
        /// <para>
        /// Expressões que não são permitidas: 
        /// <list type="bullet">
        /// <item name="Update">Update</item>
        /// <item name="Delete">Delete</item>
        /// <item name="Create">Create</item>
        /// <item name="Drop">Drop</item>
        /// <item name="Alter">Alter</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="command">Comando de inserção da ser executada</param>
        /// <param name="parameters">Parâmetros a serem introduzidos no comando a ser executado.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Quando a expressão tiver comandos não permitidos</exception>
        public static bool ExecuteInsert(string command, List<SqlParameter> parameters = null)
            => new DataHelper().ExecuteInsertStatement(expression: command, parameters);
        /// <summary>
        /// Executa o comando de atualização junto ao banco de dados.
        /// <para>
        /// Expressões que não são permitidas: 
        /// <list type="bullet">
        /// <item name="Insert">Insert</item>
        /// <item name="Delete">Delete</item>
        /// <item name="Create">Create</item>
        /// <item name="Drop">Drop</item>
        /// <item name="Alter">Alter</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="command">Comando de alteração da ser executada</param>
        /// <param name="parameters">Parâmetros a serem introduzidos no comando a ser executado.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Quando a expressão tiver comandos não permitidos</exception>
        public static bool ExecuteUpdate(string command, List<SqlParameter> parameters = null)
            => new DataHelper().ExecuteUpdateStatement(expression: command, parameters);
        /// <summary>
        /// Executa o comando de exclusão junto ao banco de dados.
        /// <para>
        /// Expressões que não são permitidas: 
        /// <list type="bullet">
        /// <item name="Insert">Insert</item>
        /// <item name="Update">Update</item>
        /// <item name="Create">Create</item>
        /// <item name="Drop">Drop</item>
        /// <item name="Alter">Alter</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="command">Comando de exclusão da ser executada</param>
        /// <param name="parameters">Parâmetros a serem introduzidos no comando a ser executado.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Quando a expressão tiver comandos não permitidos</exception>
        public static bool ExecuteDelete(string command, List<SqlParameter> parameters = null)
            => new DataHelper().ExecuteDeleteStatement(expression: command, parameters);
        /// <summary>
        /// Executa o comando junto ao banco de dados.
        /// <para>
        /// Operações que não são permitidas: 
        /// <list type="bullet">
        /// <item name="Insert">Insert</item>
        /// <item name="Update">Update</item>
        /// <item name="Delete">Delete</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="command">Comando a ser executada</param>
        /// <param name="parameters">Parâmetros a serem introduzidos no comando a ser executado.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Quando a expressão tiver comandos não permitidos</exception>
        public static int SQLStatement(string command, List<SqlParameter> parameters = null)
            => new DataHelper().ExecuteSQLStatement(expression: command, parameters);
        #endregion
    }
}
