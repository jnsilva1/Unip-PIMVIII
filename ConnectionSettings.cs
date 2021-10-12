using System;
using System.Collections.Generic;
using System.Text;

namespace CadastroPessoaFisica
{
    public class ConnectionSettings
    {
        public static void SetConnectionString(string connectionString)
            => DataHelper.SetConnectionString(connectionString: connectionString);
    }
}
