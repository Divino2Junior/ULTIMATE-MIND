using System.Net.Http;
using ULTIMATE_MIND.Arquitetura.Util;

namespace ULTIMATE_MIND.Controllers
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;

    public class ClickSignController : ControllerPadrao
    {
        private readonly HttpClient _httpClient;
        private readonly string _accessToken;

        public ClickSignController()
        {
            _accessToken = "fd498d14-e7d9-4415-a91b-901e0bec0628";
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://sandbox.clicksign.com"); // Altere para a URL de produção quando estiver em ambiente de produção
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        [HttpPost("signatarios")]
        public async Task<IActionResult> CriarSignatario([FromBody] SignatarioViewModel model)
        {
            try
            {
                var requestUri = $"/api/v1/signers?access_token={_accessToken}";

                var content = new StringContent(BuildSignerJson(model), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(requestUri, content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                return Ok(responseContent);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        private string BuildSignerJson(SignatarioViewModel model)
        {
            var jsonString = $@"
        {{
            ""signer"": {{
                ""email"": ""{model.Email}"",
                ""phone_number"": ""{model.PhoneNumber}"",
                ""auths"": [ ""api"" ],
                ""name"": ""{model.Name}"",
                ""documentation"": ""{model.Documentation}"",
                ""birthday"": ""{model.Birthday:yyyy-MM-dd}"",
                ""has_documentation"": true,
                ""selfie_enabled"": false,
                ""handwritten_enabled"": false,
                ""official_document_enabled"": false,
                ""liveness_enabled"": false,
                ""facial_biometrics_enabled"": false
            }}
        }}";

            return jsonString;
        }
    }

    public class SignatarioViewModel
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string Documentation { get; set; }
        public DateTime Birthday { get; set; }
    }

}
