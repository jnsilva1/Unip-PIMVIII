using System.Collections.Generic;

namespace CadastroPessoaFisica
{
    public class Endereco
    {
        protected internal int Id { get; set; }
        public string Logradouro { get; set; }
        public int Numero { get; set; }
        public int Cep { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Endereco endereco &&
                   Numero == endereco.Numero &&
                   Cep == endereco.Cep;
        }

        public override int GetHashCode()
        {
            int hashCode = -2144675977;
            hashCode = hashCode * -1521134295 + Numero.GetHashCode();
            hashCode = hashCode * -1521134295 + Cep.GetHashCode();
            return hashCode;
        }
    }
}