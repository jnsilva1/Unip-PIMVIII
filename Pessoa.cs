using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace CadastroPessoaFisica
{
    public class Pessoa
    {
        protected internal int Id;
        [JsonProperty("Nome",  IsReference = false)]
        public string Nome;
        [JsonProperty("Cpf", IsReference = false)]
        public long Cpf;
        [JsonProperty("Endereco", IsReference = true)]
        public Endereco Endereco;
        [JsonProperty("Telefones", IsReference = true)]
        public HashSet<Telefone> Telefones;
        public Pessoa()
        {
            this.Endereco = new Endereco();
            this.Telefones = new HashSet<Telefone>();
        }

        [JsonConstructor]
        public Pessoa(string nome, long cpf, Endereco endereco, HashSet<Telefone> telefones)
        {
            Nome = nome;
            Cpf = cpf;
            Endereco = endereco;
            Telefones = telefones;
        }
    }
}
