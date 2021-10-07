using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace CadastroPessoaFisica
{
    internal sealed class EnderecoDAO
    {
        SqlConnection Connection;

        void SetConnectionString(string connectionString)
            => Connection = new SqlConnection(connectionString: connectionString ?? throw new ArgumentNullException(paramName: nameof(connectionString)));

        public bool Exclua(Endereco e)
        {
            throw new NotImplementedException();
        }

        public bool Insira(Endereco e)
        {
            throw new NotImplementedException();
        }

        public bool Altere(Endereco e)
        {
            throw new NotImplementedException();
        }

        public Endereco Consulte(int cep)
        {
            throw new NotImplementedException();
        }

        public Endereco ObtemEndereco(int id)
        {
            throw new NotImplementedException();
        }

    }
}
