using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using ULTIMATE_MIND.Arquitetura.Util;

namespace ULTIMATE_MIND.Controllers
{
    public class ClienteController : ControllerPadrao
    {

        public ClienteController(IHostingEnvironment hostingEnvironment)
        {
            HostingEnvironment = hostingEnvironment;
        }
        public IActionResult CadastroCliente()
        {
            return View();
        }
        public IActionResult Atendimento()
        {
            return View();
        }
    }
}
