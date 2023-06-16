using System;

namespace ULTIMATE_MIND.Arquitetura.Filtros
{
    public class FiltroSalvarUsuario
    {
        public int IDUsuario { get; set; }
        public string Cpf { get; set; }
        public string Nome { get; set; }
        public int Matricula { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public int Status { get; set; }
        public int IDCargo { get; set; }
        public int IDGrupoPermissao { get; set; }
        public string Rg { get; set; }
        public string Telefone { get; set; }
        public DateTime? DataNascimento { get; set; }
        public DateTime? DataAdmissao { get; set; }
        public string ImgUsario { get; set; }
    }
}
