using System;
using System.Collections.Generic;

namespace ULTIMATE_MIND.Arquitetura.Model.UltimateMind
{
    public partial class GrupoPermissao
    {
        public GrupoPermissao()
        {
            Tela = new HashSet<Tela>();
            Usuario = new HashSet<Usuario>();
        }

        public int IdgrupoPermissao { get; set; }
        public int Idempresa { get; set; }
        public string Nome { get; set; }

        public virtual Empresa IdempresaNavigation { get; set; }
        public virtual ICollection<Tela> Tela { get; set; }
        public virtual ICollection<Usuario> Usuario { get; set; }
    }
}
