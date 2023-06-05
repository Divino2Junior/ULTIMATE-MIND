using System;
using System.Collections.Generic;

namespace ULTIMATE_MIND.Arquitetura.Model.UltimateMind
{
    public partial class Cargo
    {
        public Cargo()
        {
            Usuario = new HashSet<Usuario>();
        }

        public int Idcargo { get; set; }
        public string NomeCargo { get; set; }

        public virtual ICollection<Usuario> Usuario { get; set; }
    }
}
