using System;
using System.Collections.Generic;
using System.Text;

namespace CadastroPessoaFisica
{
    internal class DataHelper
    {
        private static string ConnectionString;
        /// <summary>
        /// Define a cadeia de conexão com o Banco
        /// </summary>
        /// <param name="connectionString">Cadeia de conexão com o banco de dados</param>
        static void SetConnectionString(string connectionString)
            => connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

    }
}
