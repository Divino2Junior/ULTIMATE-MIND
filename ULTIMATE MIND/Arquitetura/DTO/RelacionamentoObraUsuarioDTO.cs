using Microsoft.AspNetCore.Http;

namespace ULTIMATE_MIND.Arquitetura.DTO
{
    public class RelacionamentoObraUsuarioDTO
    {
        public int IdRelacionamentoObraUsuario { get; set; }
        public int IdObra { get; set; }
        public string NomeObra { get; set; }
        public int IdUsuario { get; set; }
        public string NomeUsuario { get; set; }
        public int Status { get; set; }
        public IFormFile Foto { get; set; }
        public string ImageUrl {get;set;}
    }


}
