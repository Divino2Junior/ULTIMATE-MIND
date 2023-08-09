using System;
using System.Collections.Generic;

namespace ULTIMATE_MIND.Arquitetura.Model.UltimateMind
{
    public partial class ObraUsuario
    {
        public ObraUsuario()
        {
            Atendimento = new HashSet<Atendimento>();
        }

        public int IdobraUsuario { get; set; }
        public int Idobra { get; set; }
        public int Idusuario { get; set; }

        public virtual Obra IdobraNavigation { get; set; }
        public virtual Usuario IdusuarioNavigation { get; set; }
        public virtual ICollection<Atendimento> Atendimento { get; set; }
    }
}
