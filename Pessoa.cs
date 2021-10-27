using System;
using System.Collections.Generic;
using System.Text;

namespace CadastroPessoaFisica
{
    public class Pessoa
    {
        protected internal int Id;
        public string Nome;
        public long Cpf;
        public Endereco Endereco;
        public HashSet<Telefone> Telefones;
        public Pessoa()
        {
            this.Endereco = new Endereco();
            this.Telefones = new HashSet<Telefone>();
        }
    }
}
