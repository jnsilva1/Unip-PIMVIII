using System;
using System.Collections.Generic;
using System.Text;

namespace CadastroPessoaFisica
{
    public class Telefone : IEquatable<Telefone>
    {
        protected internal int Id { get; set; }
        public int Numero { get; set; }
        public int Ddd { get; set; }
        public TipoTelefone Tipo { get; set; }

        public Telefone()
        {
            Tipo = new TipoTelefone();
        }

        

        public override bool Equals(object obj)
        {
            return Equals(obj as Telefone);
        }

        public bool Equals(Telefone other)
        {
            return other != null &&
                   Numero == other.Numero &&
                   Ddd == other.Ddd;
        }

        public override int GetHashCode()
        {
            int hashCode = -202074280;
            hashCode = hashCode * -1521134295 + Numero.GetHashCode();
            hashCode = hashCode * -1521134295 + Ddd.GetHashCode();
            return hashCode;
        }
    }
}
