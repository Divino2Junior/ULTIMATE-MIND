using System;
using System.Collections.Generic;

namespace ULTIMATE_MIND.Arquitetura.Model.UltimateMind
{
    public partial class ValidacaoContraCheque
    {
        public int IdvalidacaoContraCheque { get; set; }
        public int Idusuario { get; set; }
        public DateTime DataInserido { get; set; }
        public DateTime Referencia { get; set; }
        public string NomeArquivo { get; set; }
        public int Idempresa { get; set; }
        public bool? IsAssinado { get; set; }
        public DateTime? DataAssinado { get; set; }

        public virtual Empresa IdempresaNavigation { get; set; }
        public virtual Usuario IdusuarioNavigation { get; set; }
    }
}
