using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace CadastroPessoaFisica
{
    class TipoTelefoneDAO
    {
        SqlConnection Connection;
        /// <summary>
        /// Cria a instância do objeto responsável pelas transações no banco de dados para a tabela TELEFONE_TIPO
        /// </summary>
        /// <param name="connectionString">Cadeia de conexão com o banco de dados</param>
        public TipoTelefoneDAO(string connectionString)
            => SetConnectionString(connectionString);
        /// <summary>
        /// Cria a instância de conexão com o SQL Server
        /// </summary>
        /// <param name="connectionString">Cadeia de conexão com o banco de dados</param>
        void SetConnectionString(string connectionString)
            => Connection = new SqlConnection(connectionString: connectionString ?? throw new ArgumentNullException(paramName: nameof(connectionString)));
        /// <summary>
        /// Carrega todos os tipos de telefone cadastrados no banco de dados
        /// </summary>
        /// <returns>Lista com os tipos encontrados</returns>
        public List<TipoTelefone> Todos()
        {
            List<TipoTelefone> tipoTelefones = new List<TipoTelefone>();
            string consulta = "SELECT * FROM TELEFONE_TIPO";
            using (SqlCommand comando = new SqlCommand(cmdText: consulta, connection: Connection))
            {
                try
                {
                    if (Connection.State == System.Data.ConnectionState.Open)
                        Connection.Close();

                    Connection.Open();

                    SqlDataReader reader = comando.ExecuteReader();
                    while (reader.Read())
                    {
                        tipoTelefones.Add(new TipoTelefone { Id = Convert.ToInt32(reader["ID"]), Tipo = reader["TIPO"].ToString() });
                    }
                    reader.Close();
                }
                finally
                {
                    Connection.Close();
                }
            }
            return tipoTelefones;
        }
        /// <summary>
        /// Exclui o tipo de telefone informado
        /// </summary>
        /// <param name="tipo">Registro a ser exclido do banco</param>
        /// <returns></returns>
        public bool Exclua(TipoTelefone tipo)
        {
            bool sucedido = false;
            if (tipo != null)
            {
                if (tipo.Id > 0)
                {
                    string exclusao = "DELETE TELEFONE_TIPO WHERE ID = @ID";
                    using (SqlDataAdapter adaptador = new SqlDataAdapter(new SqlCommand(cmdText: exclusao, connection: Connection)))
                    {
                        try
                        {
                            if (Connection.State == System.Data.ConnectionState.Open)
                                Connection.Close();

                            Connection.Open();
                            adaptador.DeleteCommand.Parameters.AddWithValue(parameterName: "ID", value: tipo.Id);
                            while (adaptador.DeleteCommand.ExecuteReader().Read())
                            {
                                sucedido = true;
                            }
                        }
                        finally
                        {
                            Connection.Close();
                        }
                    }
                }
            }
            return sucedido;
        }
        /// <summary>
        /// Insere o novo registro na tabela TELEFONE_TIPO
        /// </summary>
        /// <param name="tipo">Tipo a ser inserido</param>
        /// <returns></returns>
        public bool Insira(TipoTelefone tipo)
        {
            bool sucedido = false;
            if (tipo != null)
            {
                if (tipo.Id == 0)
                {
                    string exclusao = "INSERT INTO TELEFONE_TIPO (ID, TIPO) VALUES (@ID, @TIPO)";
                    using (SqlCommand comando = new SqlCommand(cmdText: exclusao, connection: Connection))
                    {
                        try
                        {
                            if (Connection.State == System.Data.ConnectionState.Open)
                                Connection.Close();

                            Connection.Open();
                            comando.Parameters.AddWithValue(parameterName: "ID", value: tipo.Id);
                            comando.Parameters.AddWithValue(parameterName: "TIPO", value: tipo.Tipo);
                            while (comando.ExecuteReader().Read())
                            {
                                sucedido = true;
                            }
                        }
                        finally
                        {
                            Connection.Close();
                        }
                    }
                }
            }
            return sucedido;
        }
        /// <summary>
        /// Busca o tipo de telefone com base na descrição informada
        /// </summary>
        /// <param name="tipo">descrição do tipo de telefone</param>
        /// <returns>Retorna apenas um registro que satisfaça o critério de filtro</returns>
        public TipoTelefone Consulte(string tipo)
        {
            TipoTelefone retorno = null;
            //Poderia simplificar a verificação abaixo, no entanto, a forma como está, facilita ainda mais o raciocionio durante a leitura para programadores iniciantes
            if (string.IsNullOrEmpty(tipo) == false)
            {
                string exclusao = "SELECT * FROM TELEFONE_TIPO WHERE TIPO LIKE '%@TIPO%'";
                using (SqlCommand comando = new SqlCommand(cmdText: exclusao, connection: Connection))
                {
                    try
                    {
                        if (Connection.State == System.Data.ConnectionState.Open)
                            Connection.Close();

                        Connection.Open();
                        comando.Parameters.AddWithValue(parameterName: "TIPO", value: tipo);
                        SqlDataReader leitor = comando.ExecuteReader();
                        while (leitor.Read())
                        {
                            retorno = new TipoTelefone { Id = Convert.ToInt32(leitor["ID"]), Tipo = leitor["TIPO"].ToString() };
                        }
                    }
                    finally
                    {
                        Connection.Close();
                    }
                }
            }
            return retorno;
        }

        public TipoTelefone ObtemTipoTelefone(int id)
        {
            throw new NotImplementedException();
        }
    }
}
