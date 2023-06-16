using Microsoft.Net.Http.Headers;
using System.Collections.Generic;

namespace ULTIMATE_MIND.Arquitetura.DTO
{
    public class GrupoUsuarioDTO
    {
        public int IdgrupoPermissao { get; set; }
        public string NomeGrupoUsuario { get; set; }
        public List<string> lstNomeTela { get; set; }
    }
}
