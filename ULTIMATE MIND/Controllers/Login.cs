using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using ULTIMATE_MIND.Arquitetura;
using ULTIMATE_MIND.Arquitetura.DTO;
using ULTIMATE_MIND.Arquitetura.Model.UltimateMind;
using ULTIMATE_MIND.Arquitetura.Util;

namespace ULTIMATE_MIND.Controllers
{
    public class Login : ControllerPadrao
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public object Logar(UsuarioLogin usuario)
        {
            try
            {
                var context = new ultimate_mindContext();
                string bCpf = new Util().ToCpf(usuario.Cpf);

                var userBanco = context.Usuario
                    .Include(r => r.IdempresaNavigation)
                    .Where(r => r.Cpf == bCpf
                        && r.Status == EnumStatusUsuario.Ativo.ID)
                    .FirstOrDefault();

                if (userBanco != null)
                {
                    if (userBanco.Senha != new Util().GetHashMD5(usuario.Senha))
                        return this.Erro("Usuário ou senha inválidos");
                }
                else
                {
                    return this.Erro("Usuário ou senha não encontrado.");
                }

                var empresaLogon = context.Empresa.Where(r => r.Idempresa == userBanco.Idempresa).FirstOrDefault();

                if (empresaLogon == null || empresaLogon.Idempresa <= 0)
                    return this.Erro("Usuário não possui empresa padrão configurada!");

                usuario.IDUsuario = userBanco.Idusuario;
                usuario.IDEmpresaLogon = userBanco.Idempresa;
                usuario.IDGrupoUsuario = userBanco.IdgrupoPermissao;
                usuario.Nome = userBanco.Nome;
                usuario.ApelidoEmpresa = userBanco.IdempresaNavigation.Apelido;
                usuario.NomeFantasiaEmpresa = userBanco.IdempresaNavigation.NomeFantasia;
                usuario.GUID = userBanco.Guid;
                new Util().AddClaim(this, Constantes.UsuarioLogado, JsonConvert.SerializeObject(usuario));

                int listRetrno = 1;

                List<string> listaPermissoes = new List<string>();

                if (listRetrno == 1)
                {
                    Response.Cookies.Append(Constantes.EmpresaSelecionada, userBanco.Idempresa.ToString());
                    listaPermissoes.AddRange(context.Tela
                        .Where(r => r.IdgrupoPermissao == userBanco.IdgrupoPermissao && r.IdgrupoPermissaoNavigation.Idempresa == userBanco.Idempresa)
                    .Select(r => r.Idtela.ToString()));

                    new Util().AddClaim(this, Constantes.UsuarioPermissoes, JsonConvert.SerializeObject(listaPermissoes));
                }

                return listRetrno;
            }
            catch (System.Exception e)
            {
                return this.Erro(e.Message);
            }
        }
    }    
}
