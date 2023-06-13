using Microsoft.AspNetCore.Mvc;
using ULTIMATE_MIND.Arquitetura.Util;

namespace ULTIMATE_MIND.Controllers
{
    public class AdministracaoController : ControllerPadrao
    {
        public IActionResult CadastroUsuario()
        {
            return View();
        }
    }
}
