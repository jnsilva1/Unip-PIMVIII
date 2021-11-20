using Newtonsoft.Json;
using System;

namespace CadastroPessoaFisica
{
    public class TipoTelefone
    {
        protected internal int Id { get; set; }
        [JsonProperty(nameof(Tipo), IsReference = true)]
        public string Tipo { get; set; }
        public TipoTelefone()
        {

        }
        [JsonConstructor]
        public TipoTelefone(string tipo)
        {
            Tipo = tipo;
        }
    }
}
