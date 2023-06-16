using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using ULTIMATE_MIND.Arquitetura.Util;

namespace ULTIMATE_MIND.Controllers
{
    public class RelatorioController : ControllerPadrao
    {
        public RelatorioController(IHostingEnvironment hostingEnvironment)
        {
            HostingEnvironment = hostingEnvironment;
        }
        public IActionResult RelatorioAtendimento()
        {
            return View();
        }

        public IActionResult RelatorioPonto()
        {
            return View();
        }
        public IActionResult RelatorioValidacaoContraCheque()
        {
            return View();
        }
    }
}
