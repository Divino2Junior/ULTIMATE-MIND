using Microsoft.AspNetCore.Authorization;
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
                retorno.Email = usuario.Email == null ? "" : usuario.Email;
                retorno.Funcao = usuario.IdcargoNavigation.NomeCargo;

                return retorno;
            }
            catch (Exception ex)
            {
                return Erro(ex);
            }
        }
        public object GetMenu()
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
                    var telasPermitidas = context.Tela.Where(r => r.IdgrupoPermissao == usuarioLogado.IDGrupoUsuario).Select(r => r.NomeTela).ToList();


                    if (telasPermitidas.Count > 0)
                    {
                        var telasPermitidasSet = new HashSet<string>(telasPermitidas);
                        var telasVisiveis = new List<MenuItem>();

                        foreach (var item in menu.Items)
                        {
                            // Verificar se o item atual tem algum filho marcado
                            bool temFilhoMarcado = TemFilhoMarcado(item, telasPermitidasSet);

                            // Incluir o item no menu se ele está marcado ou se tem pelo menos um filho marcado
                            if (telasPermitidasSet.Contains(item.TelaId))
                            {
                                telasVisiveis.Add(item);
                            }
                            else if (temFilhoMarcado)
                            {
                                var submenuFilho = new List<MenuItem>();
                                foreach (var filho in item.Submenu)
                                {
                                    if (telasPermitidas.Contains(filho.TelaId))
                                    {
                                        submenuFilho.Add(filho);
                                    }
                                }

                                item.Submenu = new List<MenuItem>();
                                item.Submenu = submenuFilho;
                                telasVisiveis.Add(item);
                            }
                        }

                        // Retornar as informações do menu filtrado como JSON
                        return telasVisiveis;
                    }

                    // Se não tiver permissões definidas, retornar um menu vazio
                    return new { Items = new List<MenuItem>() };
                }
            }
            catch (Exception ex)
            {
                return Erro(ex); // Ou outra resposta para indicar o erro adequadamente
            }
        }
        private bool TemFilhoMarcado(MenuItem item, HashSet<string> telasPermitidasSet)
        {
            if (telasPermitidasSet.Contains(item.TelaId))
                return true;

            if (item.Submenu != null)
            {
                foreach (var submenuItem in item.Submenu)
                {
                    if (TemFilhoMarcado(submenuItem, telasPermitidasSet))
                        return true;
                }
            }

            return false;
        }

        public object ObterUsuarioLogado()
        {
            var usuario = GetUsuarioLogado();

            if (usuario == null)
                return 0;

            return usuario.IDUsuario;
        }

        public object ConsultarBtnTrocarEmpresa()
        {
            try
            {
                var context = new ultimate_mindContext();
                var userBanco = context.Usuario.Where(r => r.Idusuario == GetUsuarioLogado().IDUsuario).FirstOrDefault();
                if (userBanco != null)
                {
                    var empresas = context.EmpresaUsuario.Include(r => r.IdempresaNavigation).Where(r => r.Idusuario == userBanco.Idusuario && r.IsAtivado.Value).ToList();
                    if (empresas.Count > 0)
                    {
                        return true;
                    }
                    return null;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                return Erro(ex);
            }
        }

        [HttpPost]
        public IActionResult AlterarSenha(string senhaAtual, string novaSenha)
        {
            try
            {
                var context = new ultimate_mindContext();
                var usuarioLogado = GetUsuarioLogado();

                var usuario = context.Usuario.Where(r => r.Idusuario == usuarioLogado.IDUsuario).FirstOrDefault();

                if (usuario.Senha == new Util().GetHashMD5(senhaAtual))
                {
                    usuario.Senha = new Util().GetHashMD5(novaSenha);
                    context.Entry(usuario);
                    context.SaveChanges();
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "A senha atual é inválida." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { ex.Message });
            }
        }
    }
}

