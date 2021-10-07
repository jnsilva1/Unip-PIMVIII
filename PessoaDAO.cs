using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text;

namespace CadastroPessoaFisica
{
    public class PessoaDAO
    {
        static SqlConnection Connection;

        public static void SetConnectionString(string connectionString)
            => Connection = new SqlConnection(connectionString: connectionString ?? throw new ArgumentNullException(paramName: nameof(connectionString)));

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

        public static Pessoa Consulte(long cpf)
        {
            throw new NotImplementedException();
        }
    }
}
