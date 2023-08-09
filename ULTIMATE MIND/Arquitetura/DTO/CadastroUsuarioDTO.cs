using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ULTIMATE_MIND.Arquitetura.DTO
{
    public class CadastroUsuarioDTO
    {
        public int IdUsuario { get; set; }
        public string Matricula { get; set; }
        public string Nome { get; set; }    
        public string Cpf { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public string Rg { get; set; }
        public int Status { get; set; }
        public string NomeStatus { get; set; }
        public int IdCargo { get; set; }
        public string NomeCargo { get; set; }
        public int IdGrupoPermissao { get; set; }
        public string NomeGrupoPermissao { get; set; }
        public string DataNascimento { get; set; }
        public string DataAdmissao { get; set; }
        public string DataDemissao { get; set; }
        public IFormFile Imagem { get; set; }
        public string ImgUsuario { get; set; }
        public string HoraEntrada { get; set; }
        public string HoraSaida { get; set; }
        public string HoraInicioAlmoco { get; set; }
        public string HoraFimAlmoco { get; set; }
    }
}
