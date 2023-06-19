using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using ULTIMATE_MIND.Arquitetura.Model.UltimateMind;

namespace ULTIMATE_MIND.Arquitetura.Util
{
    public class ControllerPadrao : Controller
    {
        public IHostingEnvironment HostingEnvironment;

        public string CaminhoLog
        {
            get
            {
                var local = HostingEnvironment.WebRootPath + "/Log/";
                if (!Directory.Exists(local))
                    Directory.CreateDirectory(local);

                return local;
            }
        }

        public string CaminhoDownload
        {
            get
            {
                var local = HostingEnvironment.WebRootPath + "/Download/";
                if (!Directory.Exists(local))
                    Directory.CreateDirectory(local);

                return local;
            }
        }

        public string CaminhoImages
        {
            get
            {
                var local = HostingEnvironment.WebRootPath + "/Images/";
                if (!Directory.Exists(local))
                    Directory.CreateDirectory(local);

                return local;
            }
        }

        public string CaminhoFotoPerfil
        {
            get
            {
                var local = HostingEnvironment.WebRootPath + "/FotoPerfil/";
                if (!Directory.Exists(local))
                    Directory.CreateDirectory(local);

                return local;
            }
        }
        public string CaminhoContraCheque
        {
            get
            {
                var local = HostingEnvironment.WebRootPath + "/ContraCheques/";
                if (!Directory.Exists(local))
                    Directory.CreateDirectory(local);

                return local;
            }
        }
        public string CaminhoQrCodeCliente
        {
            get
            {
                var local = HostingEnvironment.WebRootPath + "/QrCodeCliente/";
                if (!Directory.Exists(local))
                    Directory.CreateDirectory(local);

                return local;
            }
        }
        public IActionResult Erro(Exception e)
        {
            Response.StatusCode = 300;
            return Json(new { message = e.Message });
        }

        public IActionResult Erro(string pMsg)
        {
            Response.StatusCode = 300;
            return Json(new { message = pMsg });
        }

        public IActionResult Sucesso(string pMsg)
        {
            Response.StatusCode = 200;
            return Json(new { message = pMsg });
        }
        public void SetCookies(string pNome, string pValor)
        {
            Response.Cookies.Append(pNome, pValor);
        }

        public string GetCookies(string pNome)
        {
            try
            {
                return Request.Cookies[pNome];
            }
            catch
            {
                return null;
            }
        }
        public int GetIDEmpresaLogada()
        {
            var emp = this.GetCookies(Constantes.EmpresaSelecionada);
            if (emp == null)
                return 0;
            else
                return Convert.ToInt32(emp);
        }

        public void SetIDEmpresaLogada(int pValor)
        {
            SetCookies(Constantes.EmpresaSelecionada, pValor.ToString());
        }
        public DTO.UsuarioLogin GetUsuarioLogado()
        {
            return new Util().GetUsuarioLogado(this);
        }

        public DTO.UsuarioLogin GetUsuarioLogado(ControllerBase controllerBase)
        {
            return new Util().GetUsuarioLogado(controllerBase);
        }
        public void EscreveLog(string pMsg)
        {
            EscreveLog(pMsg, "INFO");
        }

        public void EscreveLog(string pMsg, Exception ex)
        {
            EscreveLog(pMsg, "ERRO");
            EscreveLog(ex);
        }
        public void EscreveLog(Exception ex, string pInfo = "")
        {
            if (ex != null)
            {
                EscreveLog("INICIO DE TRATAMENTO ERRO", "ERRO");
                if (!string.IsNullOrEmpty(pInfo))
                    EscreveLog("INFORMAÇÃO: " + pInfo);
                EscreveLog(ex.Message, "ERRO MESSAGE: ");
                EscreveLog(ex.StackTrace, "ERRO STACKTRACE: ");
                EscreveLog(ex.ToString(), "ERRO TOSTRING: ");
                if (ex.InnerException != null)
                    EscreveLog(ex.InnerException.ToString(), "ERRO INNEREXCEPTION: ");
                if (ex.TargetSite != null)
                    EscreveLog(ex.TargetSite.ToString(), "ERRO TARGETSITE: ");
                EscreveLog("FIM DE TRATAMENTO ERRO", "ERRO");
            }
        }
        public void EscreveLog(string pMsg, string tipo)
        {
            try
            {
                string caminhoArquivo = LogPath;

                string stringDataHora = DateTime.Now.ToString("HH:mm:ss");

                TextWriter arquivo = System.IO.File.AppendText(caminhoArquivo);

                arquivo.WriteLine(stringDataHora + " " + tipo + " " + pMsg);

                arquivo.Close();
            }
            catch (Exception)
            {
            }
        }
        public string LogPath
        {
            get
            {
                var context = new ultimate_mindContext();
                var empresa = context.Empresa.Where(r=> r.Idempresa == GetIDEmpresaLogada()).FirstOrDefault();
                string folderPath = AppDomain.CurrentDomain.BaseDirectory + "\\Log\\";

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string logPath = folderPath + empresa.Apelido + "_" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                return logPath;
            }
        }
        public override ViewResult View()
        {
            ViewBag.Versao = GetType().Assembly.GetName().Version.ToString();
            ViewBag.UrlSite = GetConfiguracao()["Config:UrlPrincipal"];
            return base.View();
        }

        public override ViewResult View(object model)
        {
            ViewBag.Versao = GetType().Assembly.GetName().Version.ToString();
            ViewBag.UrlSite = GetConfiguracao()["Config:UrlPrincipal"];
            return base.View(model);
        }

        public override ViewResult View(string viewName, object model)
        {
            ViewBag.Versao = GetType().Assembly.GetName().Version.ToString();
            ViewBag.UrlSite = GetConfiguracao()["Config:UrlPrincipal"];
            return base.View(viewName, model);
        }

        public override ViewResult View(string viewName)
        {
            ViewBag.Versao = GetType().Assembly.GetName().Version.ToString();
            ViewBag.UrlSite = GetConfiguracao()["Config:UrlPrincipal"];
            return base.View(viewName);
        }
        public void SalvarArquivoDisco(IFormFile file, string pNomeArquivo, string pCaminhoLog)
        {
            if (file.Length > 0)
            {
                string filePath = Path.Combine(pCaminhoLog, pNomeArquivo);
                using FileStream fileStream = new FileStream(filePath, FileMode.Create);
                file.CopyTo(fileStream);
            }
        }
        public static IConfiguration GetConfiguracao()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

            return builder.Build();
        }

    }
}
