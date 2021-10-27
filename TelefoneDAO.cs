using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace CadastroPessoaFisica
{
    static class TelefoneDAO
    {
        /// <summary>
        /// Exclui o telefone informado do banco
        /// </summary>
        /// <param name="telefone">Telefone a ser excluído</param>
        /// <returns><see cref="bool"/> Indicando se a exclusão ocorreu com sucesso.</returns>
        public static bool Exclua(Telefone telefone)
        {
            bool resposta = false;
            if (telefone != null && PossuiVinculo(telefone: telefone) == false)
            {
                resposta = DataHelper.ExecuteDelete(
                    command: "DELETE TELEFONE WHERE ID = @ID",
                    parameters: new List<SqlParameter> {
                        new SqlParameter(parameterName: "ID", value: telefone.Id = ProximaSequencia()){DbType = DbType.Int32, SqlDbType = SqlDbType.Int },
                    });
            }
            return resposta;
        }
        /// <summary>
        /// Insere o telefone no banco de dados
        /// </summary>
        /// <param name="telefone">Telefone a ser inserido</param>
        /// <returns><see cref="bool"/> indicando se houve exito na inserção</returns>
        public static bool Insira(Telefone telefone)
        {
            bool resposta = false;
            if (telefone != null)
            {
                try
                {
                    telefone.Tipo = TipoTelefoneDAO.Consulte(tipo: telefone.Tipo.Tipo) ?? telefone.Tipo;
                    if (telefone.Tipo.Id == 0)
                        TipoTelefoneDAO.Insira(tipo: telefone.Tipo);

                    resposta = DataHelper.ExecuteInsert(
                        command: "INSERT INTO TELEFONE (ID, NUMERO, DDD, TIPO) VALUES(@ID, @NUMERO, @DDD, @TIPO)",
                        parameters: new List<SqlParameter>
                        {
                            new SqlParameter(parameterName: "ID", value: telefone.Id = ProximaSequencia()){DbType = DbType.Int32, SqlDbType = SqlDbType.Int },
                            new SqlParameter(parameterName: "NUMERO", value: telefone.Numero){DbType = DbType.Int32, SqlDbType = SqlDbType.Int},
                            new SqlParameter(parameterName: "DDD", value: telefone.Ddd){DbType = DbType.Int32, SqlDbType = SqlDbType.Int },
                            new SqlParameter(parameterName: "TIPO", value: telefone.Tipo.Id){DbType = DbType.Int32, SqlDbType = SqlDbType.Int}
                        });
                }
                catch { }
            }
            return resposta;
        }
        /// <summary>
        /// Cria o vinculo entre a pessoa e o telefone
        /// </summary>
        /// <param name="telefone">Telefone a ser vinculado a pessoa</param>
        /// <param name="pessoa">Pessoa que terá vinculo com o telefone</param>
        /// <returns><see cref="bool"/>Indicando se o vinculo foi armazenado no banco</returns>
        public static bool VinculaTelefoneComPessoa(Telefone telefone, Pessoa pessoa)
        {
            //1. Considerando que a pessoa já esteja considerada, mas nada como verificar se o id é maior que zero... 
            //2. Verifico se Telefone já possui id, se sim gero o vinculo direto, senão, solicito a inserção.
            //3. Realizo o vinculo de telefone com pessoa
            bool resposta = false;
            if (telefone != null && pessoa != null)
            {
                if (pessoa.Id > 0)
                {
                    if (telefone.Id == 0)
                        Insira(telefone: telefone);

                    resposta = DataHelper.ExecuteInsert(
                       command: "INSERT INTO PESSOA_TELEFONE (ID_PESSOA, ID_TELEFONE) VALUES(@ID_PESSOA, @ID_TELEFONE)",
                       parameters: new List<SqlParameter>
                       {
                            new SqlParameter(parameterName: "ID_PESSOA", value: pessoa.Id){DbType = DbType.Int32, SqlDbType = SqlDbType.Int },
                            new SqlParameter(parameterName: "ID_TELEFONE", value: telefone.Id){DbType = DbType.Int32, SqlDbType = SqlDbType.Int}
                       });
                }
            }
            return resposta;
        }
        /// <summary>
        /// Remove o vinculo entre a pessoa e o telefone
        /// </summary>
        /// <param name="telefone">Telefone que esta vinculado a pessoa</param>
        /// <param name="pessoa">Pessoa que tem vinculo com o telefone</param>
        /// <returns><see cref="bool"/>Indicando se o vinculo foi removido</returns>
        public static bool DesvinculaTelefoneComPessoa(Telefone telefone, Pessoa pessoa)
        {
            bool resposta = false;
            if (telefone != null && pessoa != null)
                if (pessoa.Id > 0)
                {
                    if (telefone.Id > 0)
                    {
                        resposta = DataHelper.ExecuteDelete(
                           command: "DELETE PESSOA_TELEFONE WHERE ID_PESSOA = @ID_PESSOA AND ID_TELEFONE = @ID_TELEFONE",
                           parameters: new List<SqlParameter>
                           {
                            new SqlParameter(parameterName: "ID_PESSOA", value: pessoa.Id){DbType = DbType.Int32, SqlDbType = SqlDbType.Int },
                            new SqlParameter(parameterName: "ID_TELEFONE", value: telefone.Id){DbType = DbType.Int32, SqlDbType = SqlDbType.Int}
                           });
                    }
                }
            return resposta;
        }
        /// <summary>
        /// Altera as informações do telefone como <see cref="Telefone.Ddd"/>, <see cref="Telefone.Numero"/> e <see cref="Telefone.Tipo"/>
        /// </summary>
        /// <param name="telefone">telefone a ser alterado</param>
        /// <returns><see cref="bool"/>Indicando se a alteração foi salva no banco</returns>
        public static bool Altere(Telefone telefone)
        {
            bool resposta = false;
            if (telefone != null)
                try
                {
                    if (telefone.Id > 0)
                    {
                        resposta = DataHelper.ExecuteUpdate(
                            command: "UPDATE TELEFONE SET NUMERO = @NUMERO, DDD = @DDD, TIPO = @TIPO WHERE ID = @ID",
                            parameters: new List<SqlParameter>
                            {
                            new SqlParameter(parameterName: "ID", value: telefone.Id){DbType = DbType.Int32, SqlDbType = SqlDbType.Int },
                            new SqlParameter(parameterName: "NUMERO", value: telefone.Numero){DbType = DbType.Int32, SqlDbType = SqlDbType.Int},
                            new SqlParameter(parameterName: "DDD", value: telefone.Ddd){DbType = DbType.Int32, SqlDbType = SqlDbType.Int },
                            new SqlParameter(parameterName: "TIPO", value: telefone.Tipo.Id){DbType = DbType.Int32, SqlDbType = SqlDbType.Int}
                            });
                    }
                }
                catch { }
            return resposta;
        }
        /// <summary>
        /// Realiza a consulta do telefone a partir do id informado
        /// </summary>
        /// <param name="id">Id do telefone que se quer consultar</param>
        /// <returns><see cref="Telefone"/> referência do objeto caso localizado</returns>
        public static Telefone Consulta(int id)
        {
            Telefone telefone = null;
            using (var data = DataHelper.ExecuteQuery(
                query: "SELECT * FROM TELEFONE WHERE ID = @ID",
                parameters: new List<SqlParameter> {
                    new SqlParameter(parameterName: "ID", value: id) { DbType = System.Data.DbType.Int32, SqlDbType = System.Data.SqlDbType.Int }
                }))
            {
                var rows = data.Rows;
                if (rows.Count > 0)
                    telefone = Popula(row: data.Rows[0]);
            }
            return telefone;
        }
        /// <summary>
        /// Realiza a consulta do telefone a partir do ddd e número informados
        /// </summary>
        /// <param name="ddd">Discagem direta a distância</param>
        /// <param name="numero">Número</param>
        /// <returns><see cref="Telefone"/> será null quando não localizado</returns>
        public static Telefone Consulta(int ddd, int numero)
        {
            Telefone telefone = null;
            using (var data = DataHelper.ExecuteQuery(
                query: "SELECT * FROM TELEFONE WHERE NUMERO = @NUMERO AND DDD = @DDD",
                parameters: new List<SqlParameter> {
                    new SqlParameter(parameterName: "NUMERO", value: numero){DbType = DbType.Int32, SqlDbType = SqlDbType.Int},
                    new SqlParameter(parameterName: "DDD", value: ddd){DbType = DbType.Int32, SqlDbType = SqlDbType.Int }
                }))
            {
                var rows = data.Rows;
                if (rows.Count > 0)
                    telefone = Popula(row: data.Rows[0]);
            }
            return telefone;
        }
        /// <summary>
        /// Carrega todos os telefones vinculados a pessoa informada no parâmetro
        /// </summary>
        /// <param name="pessoa">Pessoa a que se quer carregar os telefones vinculados</param>
        /// <returns>Lista de telefones vinculados a pessoa</returns>
        public static List<Telefone> TelefonesDaPessoa(Pessoa pessoa)
        {
            List<Telefone> telefones = new List<Telefone>();
            if (pessoa != null)
            {
                using (var data = DataHelper.ExecuteQuery(
                        query: "SELECT * FROM TELEFONE WHERE EXISTS (SELECT 1 FROM PESSOA_TELEFONE WHERE PESSOA_TELEFONE.ID_TELEFONE = TELEFONE.ID AND PESSOA_TELEFONE.ID_PESSOA = @ID_PESSOA)",
                        parameters: new List<SqlParameter> {
                            new SqlParameter(parameterName: "ID_PESSOA", value: pessoa.Id){DbType = DbType.Int32, SqlDbType = SqlDbType.Int }
                        }))
                {
                    DataRowCollection rows = data.Rows;
                    if (rows.Count > 0)
                        for (int index = 0; index < rows.Count; index++)
                            telefones.Add(item: Popula(row: rows[index]));
                }
            }
            return telefones;
        }
        /// <summary>
        /// Devolve a próxima sequência disponível para o id do telefone
        /// </summary>
        /// <returns></returns>
        private static int ProximaSequencia()
        {
            int sequencia = 0;
            using (var data = DataHelper.ExecuteQuery("SELECT MAX(ID) SEQUENCIA FROM TELEFONE"))
            {
                if (data.Rows.Count > 0)
                    if(data.Rows[0]["SEQUENCIA"] is DBNull == false)
                        sequencia = Convert.ToInt32(data.Rows[0]["SEQUENCIA"]);
            }
            return sequencia + 1;
        }
        /// <summary>
        /// Popula o DataRow em um objeto do tipo <see cref="Telefone"/>
        /// </summary>
        /// <param name="row">DataRow a ser populado</param>
        /// <returns><see cref="Telefone"/></returns>
        private static Telefone Popula(DataRow row)
        => row != null ? new Telefone { Id = Convert.ToInt32(value: row["ID"]), Ddd = Convert.ToInt32(value: row["DDD"]), Numero = Convert.ToInt32(value: row["NUMERO"]), Tipo = TipoTelefoneDAO.ObtemTipoTelefone(id: Convert.ToInt32(value: row["TIPO"])) } : null;
        /// <summary>
        /// Verifica se o telefone informado possui vínculo com alguma pessoa
        /// </summary>
        /// <param name="telefone">Telefone que se quer verificar o vinculo</param>
        /// <returns><see cref="bool"/> indicando se há ou não vinculo</returns>
        private static bool PossuiVinculo(Telefone telefone)
        {
            bool resposta = false;
            if(telefone != null)
            {
                try {
                    using (DataTable data = DataHelper.ExecuteQuery(
                        query: "SELECT * FROM PESSOA_TELEFONE WHERE PESSOA_TELEFONE.ID_TELEFONE = @ID_TELEFONE",
                        parameters: new List<SqlParameter> {
                            new SqlParameter(parameterName: "ID_TELEFONE", value: telefone.Id){DbType = DbType.Int32, SqlDbType = SqlDbType.Int }
                        }))
                    {
                        resposta = data.Rows.Count > 0;
                    }
                }
                catch { }
            }
            return resposta;
        }
    }
}
