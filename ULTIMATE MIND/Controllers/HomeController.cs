using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
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
                    .Include(r => r.IdcargoNavigation)
                    .Where(r => r.Idusuario == use.IDUsuario).FirstOrDefault();

                var retorno = new HomeDTO();

                var caminhoFoto = Path.Combine(CaminhoFotoPerfil, $"{usuario.Idusuario}.jpg");

                if (System.IO.File.Exists(caminhoFoto))
                {
                    retorno.Foto = "/FotoPerfil/" + $"{usuario.Idusuario}.jpg";
                }

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
        public IActionResult GetMenu()
        {
            try
            {
                var context = new ultimate_mindContext();
                var usuarioLogado = GetUsuarioLogado();
                var idempresa = GetIDEmpresaLogada();

                // Caminho do arquivo XML do menu
                var menuFilePath = Path.Combine(Directory.GetCurrentDirectory(), "menu.xml");

                // Ler o arquivo XML e deserializar para o modelo MenuModel
                var serializer = new XmlSerializer(typeof(MenuModel));
                using (var reader = new StreamReader(menuFilePath))
                {
                    var menu = (MenuModel)serializer.Deserialize(reader);

                    // Obter as permissões do usuário logado
                    var permissoesJson = HttpContext.User.Claims
                        .FirstOrDefault(c => c.Type == Constantes.UsuarioPermissoes)?.Value;

                    if (!string.IsNullOrEmpty(permissoesJson))
                    {
                        var telasPermitidas = JsonConvert.DeserializeObject<List<string>>(permissoesJson);

                        // Filtrar o menu para conter apenas as telas permitidas
                        menu.Items = menu.Items.Where(item => telasPermitidas.Contains(item.Tela)).ToArray();

                        // Retornar as informações do menu filtrado como JSON
                        return Json(new { menu.Items });
                    }

                    // Se não tiver permissões definidas, retornar um menu vazio
                    return Json(new { Items = new List<MenuItem>() });
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }); // Ou outra resposta para indicar o erro adequadamente
            }
        }

    }
}

