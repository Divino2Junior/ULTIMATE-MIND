namespace ULTIMATE_MIND.Arquitetura.DTO
{
    public class ContraChequeDTO
    {
        public int IdContraCheque { get; set; }
        public string Matricula { get; set; }
        public string NomeColaborador { get; set; }
        public string Referencia { get; set; }

        public bool? IsAssinado { get; set; }
    }
}
