using System;
using System.Collections.Generic;

namespace ULTIMATE_MIND.Arquitetura.Model.UltimateMind
{
    public partial class Ponto
    {
        public int Idponto { get; set; }
        public int Idusuario { get; set; }
        public DateTime DataPonto { get; set; }
        public DateTime? InicioDia { get; set; }
        public double? InicioDiaLat { get; set; }
        public double? InicioDiaLong { get; set; }
        public DateTime? InicioAlmoco { get; set; }
        public double? InicioAlmocoLat { get; set; }
        public double? InicioAlmocoLong { get; set; }
        public DateTime? FimAlmoco { get; set; }
        public double? FimAlmocoLat { get; set; }
        public double? FimAlmocoLong { get; set; }
        public DateTime? FimDia { get; set; }
        public double? FimDiaLat { get; set; }
        public double? FimDiaLong { get; set; }

        public virtual Usuario IdusuarioNavigation { get; set; }
    }
}
