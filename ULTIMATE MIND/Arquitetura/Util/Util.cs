using System.Text;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Newtonsoft.Json;
using ULTIMATE_MIND.Arquitetura.DTO;
using Microsoft.AspNetCore.Authentication;
using System.Text.RegularExpressions;
using System.Globalization;

namespace ULTIMATE_MIND.Arquitetura.Util
{
    public class Util
    {
        public static Util GetInstancia()
        {
            return new Util();
        }
        public string GetHashMD5(string texto)
        {
            // Cria o Hash MD5 hash
            MD5CryptoServiceProvider oMD5Provider = new MD5CryptoServiceProvider();

            // Gera o Hash Code
            byte[] bytHashCode = oMD5Provider.ComputeHash(Encoding.Default.GetBytes(texto));

            StringBuilder resultadoHash = new StringBuilder();

            for (int i = 0; i < bytHashCode.Length; i++)
                resultadoHash.Append(bytHashCode[i].ToString("x2"));

            return resultadoHash.ToString();
        }
        public string ToCpf(string pCpf)
        {
            string bCpf = "0";

            foreach (var item in pCpf.ToCharArray())
            {
                if (item == '0' || item == '1' || item == '2' || item == '3' || item == '4' || item == '5' || item == '6'
                    || item == '7' || item == '8' || item == '9')
                {
                    bCpf += item;
                }
            }

            return bCpf;
        }
        public UsuarioLogin GetUsuarioLogado(ControllerBase pBase)
        {
            var identity = (ClaimsIdentity)pBase.User.Identity;
            var calimUser = identity.Claims.Where(r => r.Type == Constantes.UsuarioLogado).FirstOrDefault();
            if (calimUser != null)
            {
                return JsonConvert.DeserializeObject<UsuarioLogin>(calimUser.Value);
            }

            return null;
        }
        public void AddClaim(ControllerBase pBase, string pKey, string pValue)
        {
            var identity = (ClaimsIdentity)pBase.User.Identity;

            var claimUp = identity.Claims.Where(r => r.Type == pKey).FirstOrDefault();
            if (claimUp != null)
                identity.RemoveClaim(claimUp);

            identity.AddClaim(new Claim(pKey, pValue));

            var claimsIdentity = new ClaimsIdentity(
                   identity.Claims, Constantes.CookieUser);

            var authProperties = new AuthenticationProperties
            {
                //AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.

                //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                // The time at which the authentication ticket expires. A
                // value set here overrides the ExpireTimeSpan option of
                // CookieAuthenticationOptions set with AddCookie.

                //IsPersistent = true,
                // Whether the authentication session is persisted across
                // multiple requests. Required when setting the
                // ExpireTimeSpan option of CookieAuthenticationOptions
                // set with AddCookie. Also required when setting
                // ExpiresUtc.

                //IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.

                //RedirectUri = <string>
                // The full path or absolute URI to be used as an http
                // redirect response value.
            };

            pBase.HttpContext.SignInAsync(
                Constantes.CookieUser,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }

        public string FormataCPF(string s)
        {
            if (s.Trim() != "")
            {
                if (s.Length > 11)
                {
                    s = s.Substring(s.Length - 11);
                }
                return string.Format(@"{0:000\.000\.000\-00}", long.Parse(s));
            }
            else
                return "";
        }

        public string FormataCNPJ(string s)
        {
            if (s.Trim() != "")
            {
                return string.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(s));
            }
            else
                return "";
        }

        public string FormataTelefone(string s)
        {
            if (s.Trim() != "")
            {
                if (s.Length > 11)
                {
                    s = s.Substring(s.Length - 11);
                }
                return string.Format(@"({0:##}) {1:####-####}", long.Parse(s.Substring(0, 2)), long.Parse(s.Substring(2)));
            }
            else
            {
                return "";
            }
        }

        public string RemoveFormatacaoCPF(string s)
        {
            // Remove todos os caracteres não numéricos do CPF
            string cpfApenasNumeros = Regex.Replace(s, @"\D", "");

            // Verifica se o CPF possui 11 dígitos
            if (cpfApenasNumeros.Length == 11)
            {
                return cpfApenasNumeros;
            }
            else
            {
                // Retorne uma string vazia ou gere uma exceção, dependendo do que for mais adequado para o seu caso
                return "";
            }
        }

        public string RemoveFormatacaoTelefone(string s)
        {
            // Remove todos os caracteres não numéricos do telefone
            string telefoneApenasNumeros = Regex.Replace(s, @"\D", "");

            // Retorna o telefone sem formatação
            return telefoneApenasNumeros;
        }
        public string RemoveFormatacaoCNPJ(string cnpj)
        {
            if (!string.IsNullOrEmpty(cnpj))
            {
                return cnpj.Replace(".", "").Replace("/", "").Replace("-", "");
            }
            else
            {
                return "";
            }
        }
        public float GetCoordenada(string pValor)
        {
            string novoValor = "";
            if (pValor.Contains("-"))
            {
                novoValor = "-";
            }

            novoValor += pValor.Replace("-", "").Replace("+", "").Replace(" ", "");

            return float.Parse(novoValor, new CultureInfo("en-US"));
        }

    }
}
