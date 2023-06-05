using System;
using System.Collections.Generic;

namespace ULTIMATE_MIND.Arquitetura.Model.UltimateMind
{
    public partial class ValidacaoContraCheque
    {
        public int IdvalidacaoContraCheque { get; set; }
        public int Idusuario { get; set; }
        public string NomeArquivo { get; set; }
        public string Referencia { get; set; }
        public bool? IsValidado { get; set; }
        public bool? IsAssinado { get; set; }

        public virtual Usuario IdusuarioNavigation { get; set; }
    }
}
