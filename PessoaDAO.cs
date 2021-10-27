using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CadastroPessoaFisica
{
    public class PessoaDAO
    {
        public static bool Exclua(Pessoa p)
        {
            bool resposta = false;
            if (p != null)
            {
                #region Etapa 1 - Excluo o vinculo com os telefones
                RemoveVinculoComTelefones(pessoa: p);
                #endregion

                #region Etapa 2 - Excluo a pessoa
                resposta = DataHelper.ExecuteDelete(
                        command:"DELETE PESSOA WHERE ID = @ID",
                        parameters: new List<SqlParameter> {
                            new SqlParameter(parameterName: "ID", value: p.Id){DbType = DbType.Int32, SqlDbType = SqlDbType.Int}
                        });
                #endregion

                #region Etapa 3 - Excluo o Endereço
                EnderecoDAO.Exclua(endereco: p.Endereco);
                #endregion
            }
            return resposta;
        }
        /// <summary>
        /// Realiza a inserção de uma nova pessoa no banco de dados
        /// </summary>
        /// <param name="p">Pessoa a ser inserida no banco de dados</param>
        /// <returns><see cref="bool"/> indicando se a inserção foi realizada com sucesso.</returns>
        public static bool Insira(Pessoa p)
        {
            bool resposta = false;
            //Só realizo a inserção se pessoa for uma referência válida, não tiver id e não existir pessoa cadastrada com o mesmo cpf
            if (p != null && p.Id == 0 && Consulte(cpf: p.Cpf) == null)
            {
                #region Etapa 1 - Cadastra o Endereço
                ValidaAlteracaoEndereco(pessoa: p);
                #endregion

                #region Etapa 2 - Cadastro a pessoa
                resposta = DataHelper.ExecuteInsert(
                    command: "INSERT INTO PESSOA (ID, NOME, CPF, ENDERECO) VALUES(@ID, @NOME, @CPF, @ENDERECO)", 
                    parameters: new List<SqlParameter> {
                        new SqlParameter(parameterName: "ID", value: p.Id = ProximaSequencia()){DbType = DbType.Int32, SqlDbType = SqlDbType.Int},
                        new SqlParameter(parameterName: "NOME", value: p.Nome){DbType = DbType.String, SqlDbType = SqlDbType.VarChar},
                        new SqlParameter(parameterName: "CPF", value: p.Cpf){DbType = DbType.Int64, SqlDbType = SqlDbType.BigInt},
                        new SqlParameter(parameterName: "ENDERECO", value: p.Endereco.Id){DbType = DbType.Int32, SqlDbType = SqlDbType.Int}
                    });
                #endregion

                #region Etapa 3 - Cadastra os telefones
                ValidaAlteracoesTelefone(pessoa: p);
                #endregion
            }
            return resposta;
        }
        /// <summary>
        /// Realiza a alteração nos dados da pessoa
        /// </summary>
        /// <param name="p">Pessoa a ser alterada</param>
        /// <returns><see cref="bool"/> indicando se a alteração foi realizada com sucesso</returns>
        public static bool Altere(Pessoa p)
        {
            bool resposta = false;
            //Só realizo a inserção se pessoa for uma referência válida, não tiver id e não existir pessoa cadastrada com o mesmo cpf
            if (p != null && p.Id > 0)
            {
                #region Etapa 1 - Cadastra o Endereço
                var ende = ValidaAlteracaoEndereco(pessoa: p);
                #endregion

                #region Etapa 2 - Cadastro a pessoa
                resposta = DataHelper.ExecuteUpdate(
                    command: "UPDATE PESSOA SET NOME = @NOME, ENDERECO = @ENDERECO WHERE ID = @ID",
                    parameters: new List<SqlParameter> {
                        new SqlParameter(parameterName: "ID", value: p.Id){DbType = DbType.Int32, SqlDbType = SqlDbType.Int},
                        new SqlParameter(parameterName: "NOME", value: p.Nome){DbType = DbType.String, SqlDbType = SqlDbType.VarChar},
                        new SqlParameter(parameterName: "ENDERECO", value: p.Endereco.Id){DbType = DbType.Int32, SqlDbType = SqlDbType.Int}
                    });
                #endregion

                #region Etapa 3 - Cadastra os telefones
                ValidaAlteracoesTelefone(pessoa: p);
                #endregion

                #region Etapa 4 - Excluo endereço anterior
                if (ende != null)
                    EnderecoDAO.Exclua(endereco: ende);
                #endregion
            }
            return resposta;
        }
        /// <summary>
        /// Consulta os dados da pessoa a partir do CPF
        /// </summary>
        /// <param name="cpf">Cpf da pessoa a ser localizada</param>
        /// <returns><see cref="Pessoa"/> será null quando não identificado</returns>
        public static Pessoa Consulte(long cpf)
        {
            Pessoa pessoa = null;
            if (cpf > 0)
            {
                using (DataTable data = DataHelper.ExecuteQuery(
                    query: "SELECT * FROM PESSOA WHERE CPF = @CPF", 
                    parameters: new List<SqlParameter> {
                         new SqlParameter(parameterName: "CPF", value: cpf){DbType = DbType.Int64, SqlDbType = SqlDbType.BigInt}
                    }))
                {
                    DataRowCollection rows = data.Rows;
                    if (rows.Count > 0)
                        pessoa = Popula(row: rows[0]);
                }
            }
            return pessoa;
        }
        /// <summary>
        /// Popula os dados da Pessoa a partir de um DataRow
        /// </summary>
        /// <param name="row">DataRow que contem os dados da pessoa</param>
        /// <returns><see cref="Pessoa"/> será null quando o parâmetro informado também for.</returns>
        private static Pessoa Popula(DataRow row)
        {
            Pessoa pessoa = null;
            if (row != null)
            {
                pessoa = new Pessoa {
                    Id = Convert.ToInt32(value: row["ID"]),
                    Nome = Convert.ToString(value: row["NOME"]),
                    Cpf = Convert.ToInt64(value: row["CPF"]),
                    Endereco = EnderecoDAO.ObtemEndereco(id: Convert.ToInt32(value: row["ENDERECO"])),
                };
                pessoa.Telefones = new HashSet<Telefone>(collection: TelefoneDAO.TelefonesDaPessoa(pessoa: pessoa));
            }
            return pessoa;
        }
        /// <summary>
        /// Remove o vinculo com os telefones e os exclui
        /// </summary>
        /// <param name="pessoa">Pessoa que se quer remover o vínculo</param>
        private static void RemoveVinculoComTelefones(Pessoa pessoa)
        {
            try
            {
                TelefoneDAO.TelefonesDaPessoa(pessoa: pessoa).ForEach(telefone => {
                    TelefoneDAO.DesvinculaTelefoneComPessoa(telefone: telefone, pessoa: pessoa);
                    TelefoneDAO.Exclua(telefone: telefone);
                });
            }
            catch { }
        }
        /// <summary>
        /// Verifica a diferência entre os telefones adicionados na coleção da pessoa atual e como estão gravados no banco. Os que estão no banco e não na coleção, serão excluído, e os que estão na coleção e não no banco, serão adicionados.
        /// </summary>
        /// <param name="pessoa">Pessoa a que terá o vinculo com os telefones validado</param>
        private static void ValidaAlteracoesTelefone(Pessoa pessoa)
        {
            if (pessoa != null)
            {
                List<Telefone> telefones = TelefoneDAO.TelefonesDaPessoa(pessoa: pessoa),
                telefonesParaExcluir = telefones.Where(tel => pessoa.Telefones.Contains(item: tel) == false).ToList(),
                telefonesParaAdicionar = pessoa.Telefones.Where(tel => telefones.Contains(item: tel) == false).ToList();

                telefonesParaExcluir.ForEach(telefone => {
                    TelefoneDAO.DesvinculaTelefoneComPessoa(telefone: telefone, pessoa: pessoa);
                    TelefoneDAO.Exclua(telefone: telefone);
                });

                telefonesParaAdicionar.ForEach(telefone => {
                    telefone = TelefoneDAO.Consulta(ddd: telefone.Ddd, numero: telefone.Numero) ?? telefone;
                    if (telefone.Id == 0)
                        TelefoneDAO.Insira(telefone: telefone);
                    else
                        TelefoneDAO.Altere(telefone: telefone);
                    TelefoneDAO.VinculaTelefoneComPessoa(telefone: telefone, pessoa: pessoa);
                });
            }
        }
        /// <summary>
        /// Verifica se a pessoa está adicionando um endereço, alterando para um novo endereço, ou apenas informações do mesmo endereço
        /// </summary>
        /// <param name="pessoa">Pessoa a que se refere o endereço</param>
        /// <returns>Endereço a ser excluido em caso de alteração</returns>
        private static Endereco ValidaAlteracaoEndereco(Pessoa pessoa)
        {

            Endereco endereco = null;
            try
            {
                Endereco enderecoAnterior = (Consulte(cpf: pessoa.Cpf)?? new Pessoa()).Endereco;
                //Se existe endereço anterior e endereço atual, continuo a verificação
                if (enderecoAnterior != null && pessoa.Endereco != null)
                {
                    bool excluirEnderecoAnterior = false;
                    //Se estou alterando o endereço, excluo o anterior.
                    if (enderecoAnterior.Equals(pessoa.Endereco) == false)
                        excluirEnderecoAnterior = true;
                    else
                        pessoa.Endereco = enderecoAnterior;//Já tenho o id, não preciso alterar nada
                    
                    //Agora verifico se devo inserir ou atualizar
                    if (pessoa.Endereco.Id == 0)
                        EnderecoDAO.Insira(endereco: pessoa.Endereco);
                    else
                        EnderecoDAO.Altere(endereco: pessoa.Endereco);

                    if (excluirEnderecoAnterior)
                        endereco = enderecoAnterior;
                }
            }
            catch { }
            return endereco;
        }
        /// <summary>
        /// Devolve a próxima sequência disponível para o id da pessoa
        /// </summary>
        /// <returns></returns>
        private static int ProximaSequencia()
        {
            int sequencia = 0;
            using (var data = DataHelper.ExecuteQuery("SELECT MAX(ID) SEQUENCIA FROM PESSOA"))
            {
                if (data.Rows.Count > 0)
                {
                    if (data.Rows[0]["SEQUENCIA"] is DBNull == false)
                        sequencia = Convert.ToInt32(data.Rows[0]["SEQUENCIA"]);
                }
            }
            return sequencia + 1;
        }
        /// <summary>
        /// Verifica no banco de dados se há alguma pessoa vinculada ao endereço informado
        /// </summary>
        /// <param name="endereco">Endereço que se quer verificar o vinculo</param>
        /// <param name="pessoa">Pessoa que deve ser desconsiderada para validação de vinculo.</param>
        /// <returns><see cref="bool"/>Indicando se há ou não vinculo</returns>
        internal static bool OutraPessoaPossuiVinculoComEndereco(Endereco endereco)
        => endereco == null ? 
            false : 
            DataHelper.ExecuteQuery(
                query: "SELECT * FROM PESSOA WHERE PESSOA.ENDERECO = @ID_ENDERECO", 
                parameters: new List<SqlParameter> { 
                    new SqlParameter(parameterName: "ID_ENDERECO", value: endereco.Id) { DbType = DbType.Int32, SqlDbType = SqlDbType.Int }
                }).Rows.Count > 0;

    }
}
