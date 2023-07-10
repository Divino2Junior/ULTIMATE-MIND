using System;
using System.Collections.Generic;

namespace ULTIMATE_MIND.Arquitetura.Model.UltimateMind
{
    public partial class Obra
    {
        public int Idobra { get; set; }
        public int Idcliente { get; set; }
        public int Status { get; set; }
        public string Endereco { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string NomeObra { get; set; }

        public virtual Cliente IdclienteNavigation { get; set; }
    }
}
