using Newtonsoft.Json;
using System.Collections.Generic;

namespace CadastroPessoaFisica
{
    public class Endereco
    {
        protected internal int Id { get; set; }
        [JsonProperty(nameof(Logradouro), IsReference = false)]
        public string Logradouro { get; set; }
        [JsonProperty(nameof(Numero), IsReference = false)]
        public int Numero { get; set; }
        [JsonProperty(nameof(Cep), IsReference = false)]
        public int Cep { get; set; }
        [JsonProperty(nameof(Bairro), IsReference = false)]
        public string Bairro { get; set; }
        [JsonProperty(nameof(Cidade), IsReference = false)]
        public string Cidade { get; set; }
        [JsonProperty(nameof(Estado), IsReference = false)]
        public string Estado { get; set; }

        public Endereco()
        {

        }

        [JsonConstructor]
        public Endereco(string logradouro, int numero, int cep, string bairro, string cidade, string estado)
        {
            Logradouro = logradouro;
            Numero = numero;
            Cep = cep;
            Bairro = bairro;
            Cidade = cidade;
            Estado = estado;
        }

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