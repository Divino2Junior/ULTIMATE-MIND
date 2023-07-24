using Microsoft.AspNetCore.Http;

namespace ULTIMATE_MIND.Arquitetura.DTO
{
    public class ObraDTO
    {
        public int Idobra { get; set; }
        public int Idcliente { get; set; }
        public string NomeCliente { get; set; }
        public int Status { get; set; }
        public string NomeStatus { get; set; }
        public string Endereco { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string NomeObra { get; set; }
    }
}
