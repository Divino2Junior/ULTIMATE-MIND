using Microsoft.AspNetCore.Mvc;
using ULTIMATE_MIND.Arquitetura.Util;

namespace ULTIMATE_MIND.Controllers
{
    public class ClienteController : ControllerPadrao
    {
        public IActionResult CadastroCliente()
        {
            return View();
        }
    }
}
