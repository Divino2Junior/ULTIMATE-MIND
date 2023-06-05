using System;
using System.Collections.Generic;

namespace ULTIMATE_MIND.Arquitetura.Model.UltimateMind
{
    public partial class Tela
    {
        public int Idtela { get; set; }
        public int IdgrupoPermissao { get; set; }
        public string NomeTela { get; set; }

        public virtual GrupoPermissao IdgrupoPermissaoNavigation { get; set; }
    }
}
