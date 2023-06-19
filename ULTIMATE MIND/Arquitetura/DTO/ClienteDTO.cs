using Microsoft.AspNetCore.Http;

namespace ULTIMATE_MIND.Arquitetura.DTO
{
    public class ClienteDTO
    {
        public int idcliente { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Cpnj { get; set; }
        public int Status { get; set; }
        public string NomeStatus { get; set; }
        public string Endereco { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public string UrlFoto { get; set; }
        public IFormFile Imagem { get; set; }
    }
}
