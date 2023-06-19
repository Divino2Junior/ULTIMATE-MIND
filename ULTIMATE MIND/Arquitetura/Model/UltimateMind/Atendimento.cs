using System;
using System.Collections.Generic;

namespace ULTIMATE_MIND.Arquitetura.Model.UltimateMind
{
    public partial class Atendimento
    {
        public int Idatendimento { get; set; }
        public int Idusuario { get; set; }
        public int Idcliente { get; set; }
        public DateTime DataAtendimento { get; set; }
        public DateTime InicioAtendimento { get; set; }
        public double InicioAtendimentoLat { get; set; }
        public double InicioAtendimentoLong { get; set; }
        public DateTime? FimAtendimento { get; set; }
        public double? FimAtendimentoLat { get; set; }
        public double? FimAtendimentoLong { get; set; }
        public string Observacao { get; set; }

        public virtual Cliente IdclienteNavigation { get; set; }
        public virtual Usuario IdusuarioNavigation { get; set; }
    }
}
