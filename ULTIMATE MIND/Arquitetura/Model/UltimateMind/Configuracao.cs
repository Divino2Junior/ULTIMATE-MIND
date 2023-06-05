using System;
using System.Collections.Generic;

namespace ULTIMATE_MIND.Arquitetura.Model.UltimateMind
{
    public partial class Configuracao
    {
        public int Idconfiguracao { get; set; }
        public int Idempresa { get; set; }
        public string Nome { get; set; }
        public string Valor { get; set; }

        public virtual Empresa IdempresaNavigation { get; set; }
    }
}
