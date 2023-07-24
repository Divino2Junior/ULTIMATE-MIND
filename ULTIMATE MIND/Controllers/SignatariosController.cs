using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ULTIMATE_MIND.Arquitetura.Model.UltimateMind;
using ULTIMATE_MIND.Arquitetura.Util;

namespace ULTIMATE_MIND.Controllers
{
    public class SignatariosController : ControllerPadrao
    {
        private const string ClicksignBaseUrl = "https://sandbox.clicksign.com/api/v1/signers";
        private const string AccessToken = "fd498d14-e7d9-4415-a91b-901e0bec0628"; // Substitua pelo seu token de acesso real

        public SignatariosController(IHostingEnvironment hostingEnvironment)
        {
            HostingEnvironment = hostingEnvironment;
        }

        public async Task<IActionResult> CriarSignatarios()
        {
            try
            {
                var _dbContext = new ultimate_mindContext();
                var idempresa = GetIDEmpresaLogada();
                var usuariosSemSignatario = _dbContext.Usuario
                    .Where(u => u.IsSignatario != true && u.ChaveSignatario == null && u.Idusuario == 2)
                    .ToList();

                foreach (var usuario in usuariosSemSignatario)
                {
                    var signatario = await CriarSignatarioClicksign(usuario);

                    if (signatario != null)
                    {
                        usuario.IsSignatario = true;
                        usuario.ChaveSignatario = signatario.key;
                        _dbContext.Entry(usuario).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        _dbContext.SaveChanges();
                    }
                }

                return Ok("Signatários criados com sucesso.");
            }
            catch (Exception ex)
            {
                return Erro("Erro ao criar signatários: " + ex.Message);
            }
        }

        private async Task<ClicksignSigner> CriarSignatarioClicksign(Usuario usuario)
        {
            var payload = new
            {
                signer = new
                {
                    email = usuario.Email,
                    phone_number = usuario.Telefone,
                    auths = new[] { "email" },
                    name = usuario.Nome,
                    documentation = usuario.Cpf,
                    birthday = usuario.DataNascimento.Value.ToString("yyyy-MM-dd"),
                    has_documentation = true,
                    selfie_enabled = false,
                    handwritten_enabled = false,
                    official_document_enabled = false,
                    liveness_enabled = false,
                    facial_biometrics_enabled = false
                }
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(ClicksignBaseUrl);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("access_token", AccessToken);

                var response = await httpClient.PostAsync("", httpContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ClicksignResponse>(responseBody);
                    return result.signer;
                }
                else
                {
                    throw new Exception($"Falha ao criar o signatário: {response.ReasonPhrase}");
                }
            }
        }

        private class ClicksignResponse
        {
            public ClicksignSigner signer { get; set; }
        }

        private class ClicksignSigner
        {
            public string key { get; set; }
        }
    }
}
