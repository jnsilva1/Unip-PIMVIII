using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace CadastroPessoaFisica
{
    class TelefoneDAO
    {
        SqlConnection Connection;

        void SetConnectionString(string connectionString)
            => Connection = new SqlConnection(connectionString: connectionString ?? throw new ArgumentNullException(paramName: nameof(connectionString)));

        public bool Exclua(Telefone t)
        {
            throw new NotImplementedException();
        }

        public bool Insira(Telefone t)
        {
            throw new NotImplementedException();
        }

        public bool Altere(Telefone t)
        {
            throw new NotImplementedException();
        }

        public Telefone Consulta(int id)
        {
            throw new NotImplementedException();
        }

        public Telefone Consulta(int ddd, int numero)
        {
            throw new NotImplementedException();
        }
    }
}
