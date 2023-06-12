using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using ULTIMATE_MIND.Arquitetura;
using ULTIMATE_MIND.Arquitetura.Filtros;
using ULTIMATE_MIND.Arquitetura.Model.UltimateMind;
using ULTIMATE_MIND.Arquitetura.Util;

namespace ULTIMATE_MIND.Controllers
{
    public class DepartamentoPessoalController : ControllerPadrao
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CadastroContraCheque()
        {
            return View();
        }

        public IActionResult ContraCheque()
        {
            return View();
        }

        public object BuscarColaboradores()
        {
            try
            {
                var context = new ultimate_mindContext();
                string q = HttpContext.Request.Query["q"].ToString();

                int idEmpresa = GetIDEmpresaLogada();

                if (string.IsNullOrEmpty(q))
                {
                    return context.Usuario.Select(r => new
                    {
                        r.Idusuario,
                        Nome = r.Matricula + " - " + r.Nome
                    });
                }

                var usuarios = context.Usuario.OrderBy(u => Convert.ToInt32(u.Matricula)).Select(r => new
                {
                    r.Idusuario,
                    Nome = r.Matricula + " - " + r.Nome
                }).ToList();

                return usuarios.Where(u => u.Nome.Normalize().Contains(q, StringComparison.OrdinalIgnoreCase)).ToList();

            }
            catch (Exception ex)
            {
                return Erro(ex);
            }

        }

        public object BuscarUsuarios()
        {
            try
            {
                var context = new ultimate_mindContext();
                var idEmpresa = GetIDEmpresaLogada();

                var users = context.Usuario.Where(r => r.Status != EnumStatusUsuario.Inativo.ID && r.Idempresa == idEmpresa).ToList();

                if (users.Count > 0)
                {
                    return users.Select(u => new
                    {
                        u.Idusuario,
                        u.Nome,
                        u.Cpf,
                        u.Matricula,
                        Funcao = u.IdcargoNavigation.NomeCargo
                    });
                }

                return null;
            }
            catch (Exception ex)
            {
                return Erro(ex);
            }


        }

        public object SalvarUsuario(string dados)
        {
            try
            {
                var context = new ultimate_mindContext();
                var idEmpresa = GetIDEmpresaLogada();
                var filtro = JsonConvert.DeserializeObject<FiltroSalvarUsuario>(dados);

                if (filtro.IDUsuario == 0)
                {
                    var user = new Usuario();
                    user.Idempresa = idEmpresa;
                    user.Nome = filtro.Nome;
                    user.Matricula = filtro.Matricula.ToString();
                    user.Cpf = filtro.Cpf;
                    user.Senha = new Util().GetHashMD5(filtro.Cpf.Substring(0, 4));
                    user.Idcargo = filtro.IDCargo;
                    user.Email = filtro.Email;
                    user.DataNascimento = filtro.DataNascimento;
                    user.Status = filtro.Status;
                    user.Telefone = filtro.Telefone;
                    user.Rg = filtro.Rg;
                    user.IdgrupoPermissao = filtro.IDGrupoPermissao;
                    context.Add(user);
                    context.SaveChanges();
                }
                else
                {
                    var usuario = context.Usuario.Where(r => r.Idusuario == filtro.IDUsuario).FirstOrDefault();
                    if (usuario != null)
                    {
                        if (usuario.Nome != filtro.Nome)
                            usuario.Nome = filtro.Nome;
                        if (usuario.Senha != filtro.Senha)
                            usuario.Senha = new Util().GetHashMD5(filtro.Senha.Substring(0, 4));
                        if (usuario.Matricula != filtro.Matricula.ToString())
                            usuario.Matricula = filtro.Matricula.ToString();
                        if (usuario.Cpf != filtro.Cpf)
                            usuario.Cpf = filtro.Cpf;
                        if (usuario.Idcargo != filtro.IDCargo)
                            usuario.Idcargo = filtro.IDCargo;
                        if (usuario.Email != filtro.Email)
                            usuario.Email = filtro.Email;
                        if (usuario.DataNascimento != filtro.DataNascimento)
                            usuario.DataNascimento = filtro.DataNascimento;
                        if (usuario.Status != filtro.Status)
                            usuario.Status = filtro.Status;
                        if (usuario.Telefone != filtro.Telefone)
                            usuario.Telefone = filtro.Telefone;
                        if (usuario.Rg != filtro.Rg)
                            usuario.Rg = filtro.Rg;
                        if (usuario.IdgrupoPermissao != filtro.IDGrupoPermissao)
                            usuario.IdgrupoPermissao = filtro.IDGrupoPermissao;

                        context.Entry(usuario);
                        context.SaveChanges();
                    }
                    else
                        return Erro("Usuário não encontrado. Procure o suporte!!");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return Erro(ex);
            }
        }

        public IActionResult InserirContraCheque(IFormFile arquivoPdf, string usuario, DateTime mesReferencia)
        {
            try
            {
                // Lógica para inserir o contra cheque no banco de dados ou fazer outras operações

                // ...

                // Dados atualizados do DataTable
                var dataTableData = "teste"; // Implemente seu próprio método para obter os dados atualizados

                // Mensagem de sucesso
                var message = "Contra-Cheque inserido com sucesso.";

                // Retorne os dados e a mensagem em um objeto JSON
                var result = new { data = dataTableData, message = message };
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Em caso de erro, retorne uma mensagem de erro
                return BadRequest("Ocorreu um erro ao inserir o Contra-Cheque: " + ex.Message);
            }
        }

        public object BuscarContraCheque()
        {
            return null;
        }
    }
}
