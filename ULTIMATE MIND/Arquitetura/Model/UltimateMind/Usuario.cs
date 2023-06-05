using System;
using System.Collections.Generic;

namespace ULTIMATE_MIND.Arquitetura.Model.UltimateMind
{
    public partial class Usuario
    {
        public Usuario()
        {
            Atendimento = new HashSet<Atendimento>();
            Ponto = new HashSet<Ponto>();
            ValidacaoContraCheque = new HashSet<ValidacaoContraCheque>();
        }

        public int Idusuario { get; set; }
        public int Idempresa { get; set; }
        public string Nome { get; set; }
        public string Matricula { get; set; }
        public string Cpf { get; set; }
        public int Idcargo { get; set; }
        public string Email { get; set; }
        public DateTime? DataNascimento { get; set; }
        public int Status { get; set; }
        public string Telefone { get; set; }
        public string Rg { get; set; }
        public int IdgrupoPermissao { get; set; }
        public string Senha { get; set; }
        public DateTime? DataAdmissao { get; set; }

        public virtual Cargo IdcargoNavigation { get; set; }
        public virtual Empresa IdempresaNavigation { get; set; }
        public virtual GrupoPermissao IdgrupoPermissaoNavigation { get; set; }
        public virtual ICollection<Atendimento> Atendimento { get; set; }
        public virtual ICollection<Ponto> Ponto { get; set; }
        public virtual ICollection<ValidacaoContraCheque> ValidacaoContraCheque { get; set; }
    }
}
