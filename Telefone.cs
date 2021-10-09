using System;
using System.Collections.Generic;
using System.Text;

namespace CadastroPessoaFisica
{
    public class Telefone
    {
        protected internal int Id { get; set; }
        public int Numero { get; set; }
        public int Ddd { get; set; }
        public TipoTelefone Tipo { get; set; }

        public override int GetHashCode()
        {
            return 2108858624 + Id.GetHashCode();
        }
    }
}
