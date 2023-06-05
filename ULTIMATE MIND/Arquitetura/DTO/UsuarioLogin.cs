using System;

namespace ULTIMATE_MIND.Arquitetura.DTO
{
    public class UsuarioLogin
    {
        public string Cpf { get; set; }
        public string Senha { get; set; }
        public int IDUsuario { get; set; }
        public int IDGrupoUsuario { get; set; }
        public int IDEmpresaLogon { get; set; }
        public string Nome { get; set; }
        public string ApelidoEmpresa { get; set; }
        public string NomeFantasiaEmpresa { get; set; }
        public string GUID { get; set; }

        public int TipoFuncao { get; set; }
        public DateTime? DataHora { get; set; }
    }
}
