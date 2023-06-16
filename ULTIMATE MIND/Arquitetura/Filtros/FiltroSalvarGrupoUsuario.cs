using System.Collections.Generic;

namespace ULTIMATE_MIND.Arquitetura.Filtros
{
    public class FiltroSalvarGrupoUsuario
    {
        public int IDGrupoPermissao { get; set; }
        public string NomeGrupoUsuario { get; set; }
        public List<string> ListaTela { get; set; }
    }
}
