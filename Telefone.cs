using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CadastroPessoaFisica
{
    public class Telefone : IEquatable<Telefone>
    {
        protected internal int Id { get; set; }
        [JsonProperty(nameof(Numero), IsReference = false)]
        public int Numero { get; set; }
        [JsonProperty(nameof(Ddd), IsReference = false)]
        public int Ddd { get; set; }
        [JsonProperty(nameof(Tipo), IsReference = true)]
        public TipoTelefone Tipo { get; set; }

        public Telefone()
        {
            Tipo = new TipoTelefone();
        }

        [JsonConstructor]
        public Telefone(int numero, int ddd, TipoTelefone tipo)
        {
            Numero = numero;
            Ddd = ddd;
            Tipo = tipo;
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
