using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ULTIMATE_MIND.Arquitetura.DTO;
using ULTIMATE_MIND.Arquitetura.Enum;
using ULTIMATE_MIND.Arquitetura.Model.UltimateMind;
using ULTIMATE_MIND.Arquitetura.Util;
using ULTIMATE_MIND.Models;

namespace ULTIMATE_MIND.Controllers
{
    public class HomeController : ControllerPadrao
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IHostingEnvironment hostingEnvironment)
        {
            _logger = logger;
            HostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public object BuscarInfoHome()
        {
            try
            {
                var context = new ultimate_mindContext();
                var use = GetUsuarioLogado();

                var usuario = context.Usuario
                    .Include(r=> r.IdcargoNavigation)
                    .Where(r => r.Idusuario == use.IDUsuario).FirstOrDefault();

                var retorno = new HomeDTO();
                retorno.Foto = this.CaminhoFotoPerfil + usuario.NomeFoto;
                retorno.Matricula = usuario.Matricula;
                retorno.Nome = usuario.Nome;
                retorno.Cpf = new Util().FormataCPF(usuario.Cpf);
                retorno.Status = EnumStatusUsuario.Obtenha(usuario.Status);
                retorno.Email = usuario.Email;
                retorno.Funcao = usuario.IdcargoNavigation.NomeCargo;

                return retorno;
            }
            catch (Exception ex)
            {
                return Erro(ex);
            }
        }
    }
}
