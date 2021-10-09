using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace CadastroPessoaFisica
{
    class TipoTelefoneDAO
    {
        /// <summary>
        /// Carrega todos os tipos de telefone cadastrados no banco de dados
        /// </summary>
        /// <returns>Lista com os tipos encontrados</returns>
        public List<TipoTelefone> Todos()
        {
            List<TipoTelefone> tipoTelefones = new List<TipoTelefone>();
            string consulta = "SELECT * FROM TELEFONE_TIPO";
            using (var data = DataHelper.ExecuteQuery(query: consulta))
            {
                if(data.Rows.Count > 0)
                {
                    var rows = data.Rows;
                    for(int index = 0; index < rows.Count; index++)
                        tipoTelefones.Add(new TipoTelefone { Id = Convert.ToInt32(rows[index]["ID"]), Tipo = rows[index]["TIPO"].ToString() });
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
                    sucedido = DataHelper.ExecuteDelete(
                        command: "DELETE TELEFONE_TIPO WHERE ID = @ID", 
                        parameters: new List<SqlParameter> { 
                               new SqlParameter(parameterName: "ID", value: tipo.Id) { DbType = System.Data.DbType.Int32, SqlDbType = System.Data.SqlDbType.Int } 
                            }
                        );
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
                    sucedido = DataHelper.ExecuteInsert(
                        command: "INSERT INTO TELEFONE_TIPO (ID, TIPO) VALUES (@ID, @TIPO)",
                        parameters: new List<SqlParameter> {
                            new SqlParameter(parameterName: "ID", value: tipo.Id) { DbType = System.Data.DbType.Int32, SqlDbType = System.Data.SqlDbType.Int },
                            new SqlParameter(parameterName: "TIPO", value: tipo.Tipo) { DbType = System.Data.DbType.String, SqlDbType = System.Data.SqlDbType.VarChar }
                        });
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
                using (var data = DataHelper.ExecuteQuery(
                    query: "SELECT * FROM TELEFONE_TIPO WHERE TIPO LIKE '%@TIPO%'",
                    parameters: new List<SqlParameter> {
                        new SqlParameter(parameterName: "TIPO", value: tipo) { DbType = System.Data.DbType.String, SqlDbType = System.Data.SqlDbType.VarChar }
                    }))
                {
                    if (data.Rows.Count > 0)
                    {
                        retorno = new TipoTelefone { Id = Convert.ToInt32(data.Rows[0]["ID"]), Tipo = data.Rows[0]["TIPO"].ToString() };
                    }
                }
            }
            return retorno;
        }
        /// <summary>
        /// Busca o tipo de telefone com base no id informado
        /// </summary>
        /// <param name="id">Id do tipo de telefone</param>
        /// <returns><see cref="TipoTelefone"/></returns>
        public TipoTelefone ObtemTipoTelefone(int id)
        {
            TipoTelefone retorno = null;
            //Poderia simplificar a verificação abaixo, no entanto, a forma como está, facilita ainda mais o raciocionio durante a leitura para programadores iniciantes
            using (var data = DataHelper.ExecuteQuery(
                query: "SELECT * FROM TELEFONE_TIPO WHERE ID = @ID",
                parameters: new List<SqlParameter> {
                    new SqlParameter(parameterName: "ID", value: id) { DbType = System.Data.DbType.Int32, SqlDbType = System.Data.SqlDbType.Int }
                }))
            {
                if (data.Rows.Count > 0)
                {
                    retorno = new TipoTelefone { Id = Convert.ToInt32(data.Rows[0]["ID"]), Tipo = data.Rows[0]["TIPO"].ToString() };
                }
            }
            return retorno;
        }
    }
}
