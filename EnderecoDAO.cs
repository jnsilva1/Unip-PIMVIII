using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace CadastroPessoaFisica
{
    static class EnderecoDAO
    {
        /// <summary>
        /// Exclui o endereço informado do banco
        /// </summary>
        /// <param name="endereco">Endereço a ser excluído</param>
        /// <returns><see cref="bool"/> indicando se a exclusão foi realizada com sucesso</returns>
        public static bool Exclua(Endereco endereco)
        {
            bool resposta = false;
            //´Só realizo a exclusão de endereço for uma referência válida, e tiver id e não tiver vinculo com outras pessoas
            if (endereco != null && endereco.Id > 0 && PessoaDAO.OutraPessoaPossuiVinculoComEndereco(endereco: endereco) == false)
            {
                resposta = DataHelper.ExecuteDelete(
                        command: "DELETE ENDERECO WHERE ID = @ID",
                        parameters: new List<SqlParameter> {
                             new SqlParameter(parameterName: "ID", value: endereco.Id){DbType = DbType.Int32, SqlDbType = SqlDbType.Int }
                        });
            }
            return resposta;
        }
        /// <summary>
        /// Insere um novo endereço no banco de dados
        /// </summary>
        /// <param name="endereco">Endereço a ser inserido</param>
        /// <returns><see cref="bool"/> indicando se a inserção foi realizada com sucesso</returns>
        public static bool Insira(Endereco endereco)
        {
            bool resposta = false;
            if (endereco != null && endereco.Id == 0)
            {
                resposta = DataHelper.ExecuteInsert(
                        command: "INSERT INTO ENDERECO (ID, LOGRADOURO, NUMERO, CEP, BAIRRO, CIDADE, ESTADO) VALUES(@ID, @LOGRADOURO, @NUMERO, @CEP, @BAIRRO, @CIDADE, @ESTADO)",
                        parameters: new List<SqlParameter> {
                             new SqlParameter(parameterName: "ID", value: endereco.Id = ProximaSequencia()){DbType = DbType.Int32, SqlDbType = SqlDbType.Int },
                             new SqlParameter(parameterName: "LOGRADOURO", value: endereco.Logradouro){DbType = DbType.String, SqlDbType = SqlDbType.VarChar},
                             new SqlParameter(parameterName: "NUMERO", value: endereco.Numero){DbType = DbType.Int32, SqlDbType = SqlDbType.Int },
                             new SqlParameter(parameterName: "CEP", value: endereco.Cep){DbType = DbType.Int32, SqlDbType = SqlDbType.Int },
                             new SqlParameter(parameterName: "BAIRRO", value: endereco.Bairro){DbType = DbType.String, SqlDbType = SqlDbType.VarChar},
                             new SqlParameter(parameterName: "CIDADE", value: endereco.Cidade){DbType = DbType.String, SqlDbType = SqlDbType.VarChar},
                             new SqlParameter(parameterName: "ESTADO", value: endereco.Estado){DbType = DbType.String, SqlDbType = SqlDbType.VarChar}
                        });
            }
            return resposta;
        }
        /// <summary>
        /// Salva as alterações feitas em <see cref="Endereco.Logradouro"/> e <see cref="Endereco.Numero"/>
        /// </summary>
        /// <param name="endereco">Endereço a ser atualizado no banco</param>
        /// <returns><see cref="bool"/> indicando se as alterações foram gravadas com sucesso</returns>
        public static bool Altere(Endereco endereco)
        {
            bool resposta = false;
            if (endereco != null && endereco.Id > 0)
            {
                resposta = DataHelper.ExecuteUpdate(
                        command: "UPDATE ENDERECO SET LOGRADOURO = @LOGRADOURO, NUMERO = @NUMERO WHERE  ID = @ID",
                        parameters: new List<SqlParameter> {
                             new SqlParameter(parameterName: "ID", value: endereco.Id){DbType = DbType.Int32, SqlDbType = SqlDbType.Int },
                             new SqlParameter(parameterName: "LOGRADOURO", value: endereco.Logradouro){DbType = DbType.String, SqlDbType = SqlDbType.VarChar},
                             new SqlParameter(parameterName: "NUMERO", value: endereco.Numero){DbType = DbType.Int32, SqlDbType = SqlDbType.Int }
                        });
            }
            return resposta;
        }
        /// <summary>
        /// Consulta o endereço a partir do CEP
        /// </summary>
        /// <param name="cep">Cep a localizar o endereço</param>
        /// <returns><see cref="Endereco"/> será null se não encontrado.</returns>
        public static Endereco Consulte(int cep)
        {
            Endereco endereco = null;
            using (DataTable data = DataHelper.ExecuteQuery( 
                query: "SELECT * FROM ENDERECO WHERE CEP = @CEP",
                parameters: new List<SqlParameter> {
                    new SqlParameter(parameterName: "CEP", value: cep){DbType = DbType.Int32, SqlDbType = SqlDbType.Int },
                }))
            {
                DataRowCollection rows = data.Rows;
                if (rows.Count > 0)
                    endereco = Popula(row: rows[0]);
            }
            return endereco;
        }
        /// <summary>
        /// Busca o endereço a partir do id
        /// </summary>
        /// <param name="id">Id do endereço a ser consultado</param>
        /// <returns><see cref="Endereco"/> será null quando não localizado</returns>
        public static Endereco ObtemEndereco(int id)
        {
            Endereco endereco = null;
            using (DataTable data = DataHelper.ExecuteQuery(
                query: "SELECT * FROM ENDERECO WHERE ID = @ID",
                parameters: new List<SqlParameter> {
                    new SqlParameter(parameterName: "ID", value: id){DbType = DbType.Int32, SqlDbType = SqlDbType.Int },
                }))
            {
                DataRowCollection rows = data.Rows;
                if (rows.Count > 0)
                    endereco = Popula(row: rows[0]);
            }
            return endereco;
        }
        /// <summary>
        /// Devolve a próxima sequência disponível para o id do endereço
        /// </summary>
        /// <returns></returns>
        private static int ProximaSequencia()
        {
            int sequencia = 0;
            using (var data = DataHelper.ExecuteQuery("SELECT MAX(ID) SEQUENCIA FROM ENDERECO"))
            {
                if (data.Rows.Count > 0)
                    sequencia = Convert.ToInt32(data.Rows[0]["SEQUENCIA"]);
            }
            return sequencia + 1;
        }
        /// <summary>
        /// Popula os dados de endereço contido em um DataRow para um objeto do tipo <see cref="Endereco"/>
        /// </summary>
        /// <param name="row">Objeto que representa uma linha do resultado de consulta em banco de dados</param>
        /// <returns><see cref="Endereco"/> será null quando o parâmetro informado também for</returns>
        private static Endereco Popula(DataRow row)
        {
            Endereco endereco = null;
            if (row != null)
            {
                endereco = new Endereco { 
                    Id = Convert.ToInt32(row["ID"]), 
                    Logradouro = row["LOGRADOURO"].ToString(),
                    Numero = Convert.ToInt32(row["NUMERO"]),
                    Cep = Convert.ToInt32(row["CEP"]),
                    Bairro = Convert.ToString(row["BAIRRO"]),
                    Cidade = Convert.ToString(row["CIDADE"]),
                    Estado = Convert.ToString(row["ESTADO"])
                };
            }
            return endereco;
        }
    }
}
