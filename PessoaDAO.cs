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
            throw new NotImplementedException();
        }

        public static bool Insira(Pessoa p)
        {
            throw new NotImplementedException();
        }

        public static bool Altere(Pessoa p)
        {
            throw new NotImplementedException();
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
                    if (telefone.Id == 0)
                        TelefoneDAO.Insira(telefone: telefone);
                    else
                        TelefoneDAO.Altere(telefone: telefone);
                    TelefoneDAO.VinculaTelefoneComPessoa(telefone: telefone, pessoa: pessoa);
                });
            }
        }
    }
}
