using System;
using System.Collections.Generic;

namespace ULTIMATE_MIND.Arquitetura.Model.UltimateMind
{
    public partial class HistoricoLiberacaoPonto
    {
        public int IdhistoricoLiberacaoPonto { get; set; }
        public int IdusuarioGestor { get; set; }
        public int IdusuarioLiberado { get; set; }
        public DateTime DataHoraLiberacao { get; set; }
        public string ObservacaoLiberacao { get; set; }

        public virtual Usuario IdusuarioGestorNavigation { get; set; }
        public virtual Usuario IdusuarioLiberadoNavigation { get; set; }
    }
}
