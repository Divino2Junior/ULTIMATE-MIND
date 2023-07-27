using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using ULTIMATE_MIND.Arquitetura.DTO;
using ULTIMATE_MIND.Arquitetura.Enum;
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

        public Login(IHostingEnvironment hostingEnvironment)
        {
            HostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        public object Logar(UsuarioLogin usuario)
        {
            try
            {
                var context = new ultimate_mindContext();
                string bCpf = new Util().RemoveFormatacaoCPF(usuario.Cpf);

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

                var empresas = context.EmpresaUsuario.Include(r=> r.IdempresaNavigation).Where(r => r.Idusuario == userBanco.Idusuario && r.IsAtivado.Value).ToList();

                if (empresas.Count > 1)
                {
                    usuario.IDUsuario = userBanco.Idusuario;
                    usuario.IDGrupoUsuario = userBanco.IdgrupoPermissao;
                    usuario.Nome = userBanco.Nome;
                    new Util().AddClaim(this, Constantes.UsuarioLogado, JsonConvert.SerializeObject(usuario));
                    return empresas.Select(r => new { r.Idempresa, r.IdempresaNavigation.Apelido }).ToList();

                }

                usuario.IDUsuario = userBanco.Idusuario;
                usuario.IDEmpresaLogon = userBanco.Idempresa;
                usuario.IDGrupoUsuario = userBanco.IdgrupoPermissao;
                usuario.Nome = userBanco.Nome;
                usuario.ApelidoEmpresa = userBanco.IdempresaNavigation.Apelido;
                usuario.NomeFantasiaEmpresa = userBanco.IdempresaNavigation.NomeFantasia;
                new Util().AddClaim(this, Constantes.UsuarioLogado, JsonConvert.SerializeObject(usuario));

                int listRetrno = 1;

                List<string> listaPermissoes = new List<string>();

                if (listRetrno == 1)
                {
                    Response.Cookies.Append(Constantes.EmpresaSelecionada, userBanco.Idempresa.ToString());
                    listaPermissoes.AddRange(context.Tela
                        .Where(r => r.IdgrupoPermissao == userBanco.IdgrupoPermissao && r.IdgrupoPermissaoNavigation.Idempresa == userBanco.Idempresa)
                    .Select(r => r.NomeTela));

                    SetCookies(Constantes.EmpresaSelecionada, userBanco.Idempresa.ToString());

                    new Util().AddClaim(this, Constantes.UsuarioPermissoes, JsonConvert.SerializeObject(listaPermissoes));
                }

                return listRetrno;
            }
            catch (System.Exception e)
            {
                return this.Erro(e.Message);
            }
        }

        public List<Tela> ObterPermissoesDoUsuario(int userId)
        {
            using (var dbContext = new ultimate_mindContext())
            {
                // Obter o grupo de permissões do usuário
                var grupoPermissao = dbContext.Usuario
                    .Include(u => u.IdgrupoPermissaoNavigation)
                    .Where(u => u.Idusuario == userId)
                    .Select(u => u.IdgrupoPermissaoNavigation)
                    .FirstOrDefault();

                if (grupoPermissao != null)
                {
                    // Obter as telas associadas ao grupo de permissões
                    var permissoes = dbContext.Tela
                        .Where(t => t.IdgrupoPermissao == grupoPermissao.IdgrupoPermissao)
                        .ToList();

                    return permissoes;
                }

                return new List<Tela>(); // Retornar uma lista vazia caso o usuário não tenha um grupo de permissões associado
            }
        }

        [HttpPost]
        public object SalvarEmpresaLogin(int id)
        {
            try
            {
                var context = new ultimate_mindContext();

                var empresa = context.Empresa.Where(r => r.Idempresa == id).FirstOrDefault();

                var usuario = GetUsuarioLogado();
                usuario.IDEmpresaLogon = empresa.Idempresa;
                usuario.ApelidoEmpresa = empresa.Apelido;
                usuario.NomeFantasiaEmpresa = empresa.NomeFantasia;
                new Util().AddClaim(this, Constantes.UsuarioLogado, JsonConvert.SerializeObject(usuario));

                SetCookies(Constantes.EmpresaSelecionada, empresa.Idempresa.ToString());

                return Ok();

            }
            catch (Exception ex)
            {
                return Erro(ex);
            }
        }
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();

            return Ok();
        }


        public object BuscarSelectEmpresa()
        {
            try
            {
                var context = new ultimate_mindContext();
                string q = HttpContext.Request.Query["q"].ToString();

                var user = GetUsuarioLogado();

                if (string.IsNullOrEmpty(q))
                {
                    return context.EmpresaUsuario
                        .Include(r=> r.IdusuarioNavigation).Where(r=> r.Idusuario == user.IDUsuario && r.IsAtivado.Value).Select(r => new
                    {
                        r.Idempresa,
                        Nome = r.IdempresaNavigation.Apelido
                    }).ToList();
                }

                var empresa = context.EmpresaUsuario
                        .Include(r => r.IdusuarioNavigation).Where(r => r.Idusuario == user.IDUsuario && r.IsAtivado.Value).OrderBy(u => u.Idempresa).Select(r => new
                        {
                            r.Idempresa,
                            Nome = r.IdempresaNavigation.Apelido
                        }).ToList();

                return empresa.Where(u => u.Nome.Normalize().Contains(q, StringComparison.OrdinalIgnoreCase)).ToList();

            }
            catch (Exception ex)
            {
                return Erro(ex);
            }

        }
    }
}
