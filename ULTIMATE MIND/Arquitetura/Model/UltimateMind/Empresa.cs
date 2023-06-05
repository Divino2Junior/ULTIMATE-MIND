using System;
using System.Collections.Generic;

namespace ULTIMATE_MIND.Arquitetura.Model.UltimateMind
{
    public partial class Empresa
    {
        public Empresa()
        {
            Cliente = new HashSet<Cliente>();
            Configuracao = new HashSet<Configuracao>();
            GrupoPermissao = new HashSet<GrupoPermissao>();
            Usuario = new HashSet<Usuario>();
        }

        public int Idempresa { get; set; }
        public string RazaoSocial { get; set; }
        public string NomeFantasia { get; set; }
        public string Cnpj { get; set; }
        public int Status { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Endereco { get; set; }
        public string Telefone { get; set; }
        public string Apelido { get; set; }

        public virtual ICollection<Cliente> Cliente { get; set; }
        public virtual ICollection<Configuracao> Configuracao { get; set; }
        public virtual ICollection<GrupoPermissao> GrupoPermissao { get; set; }
        public virtual ICollection<Usuario> Usuario { get; set; }
    }
}
