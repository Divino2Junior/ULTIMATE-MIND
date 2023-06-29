using System;
using System.Collections.Generic;

namespace ULTIMATE_MIND.Arquitetura.Model.UltimateMind
{
    public partial class EmpresaUsuario
    {
        public int IdempresaUsuario { get; set; }
        public int Idempresa { get; set; }
        public int Idusuario { get; set; }
        public bool? IsAtivado { get; set; }

        public virtual Empresa IdempresaNavigation { get; set; }
        public virtual Usuario IdusuarioNavigation { get; set; }
    }
}
