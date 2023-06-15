using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using ULTIMATE_MIND.Arquitetura.DTO;
using ULTIMATE_MIND.Arquitetura.Enum;
using ULTIMATE_MIND.Arquitetura.Model.UltimateMind;
using ULTIMATE_MIND.Arquitetura.Util;

namespace ULTIMATE_MIND.Controllers
{
    public class AdministracaoController : ControllerPadrao
    {
        public IActionResult CadastroUsuario()
        {
            return View();
        }
        public IActionResult CadastroPermissao()
        {
            return View();
        }
        public IActionResult Empresa()
        {
            return View();
        }

        public object BuscarUsuarios()
        {
            try
            {
                var context = new ultimate_mindContext();
                var empresa = 1;

                var usuarios = context.Usuario.Where(r => r.Idempresa == empresa && r.Status != EnumStatusUsuario.Inativo.ID)
                    .Include(r => r.IdcargoNavigation)
                    .Select(r => new
                    {
                        r.Idusuario,
                        Matricula = Convert.ToInt32(r.Matricula),
                        r.Status,
                        r.Nome,
                        Cargo = r.IdcargoNavigation.NomeCargo,
                        Cpf = new Util().FormataCPF(r.Cpf),
                    }).ToList();

                if (usuarios.Count > 0)
                    return usuarios.OrderBy(r=> r.Matricula).ToList();

                return null;
            }
            catch (Exception ex)
            {
                return Erro(ex);
            }
        }

        public object BuscarInfoUsuario(int id)
        {
            try
            {
                var context = new ultimate_mindContext();
                var usuario = context.Usuario
                    .Include(r=> r.IdcargoNavigation)
                    .Include(r=> r.IdgrupoPermissaoNavigation)
                    .Where(r => r.Idusuario == id).FirstOrDefault();

                if (usuario == null)
                    return Erro("Usuário não encontrado!!");

                var retorno = new CadastroUsuarioDTO();

                retorno.IdUsuario = usuario.Idusuario;
                retorno.Matricula = usuario.Matricula;
                retorno.Nome = usuario.Nome;
                retorno.Cpf = new Util().FormataCPF(usuario.Cpf);
                retorno.Telefone = usuario.Telefone == null ? "" : new Util().FormataTelefone(usuario.Telefone);
                retorno.Email = usuario.Email ?? "";
                retorno.Rg = usuario.Rg ?? "";
                retorno.Status = usuario.Status;
                retorno.NomeStatus = EnumStatusUsuario.Obtenha(usuario.Status);
                retorno.IdCargo = usuario.Idcargo;
                retorno.NomeCargo = usuario.IdcargoNavigation.NomeCargo;
                retorno.DataNascimento = usuario.DataNascimento == null ? "" : usuario.DataNascimento.Value.ToString("yyyy-MM-dd");
                retorno.DataAdmissao = usuario.DataAdmissao == null ? "" : usuario.DataAdmissao.Value.ToString("yyyy-MM-dd");
                retorno.DataDemissao = usuario.DataDemissao == null ? "" : usuario.DataDemissao.Value.ToString("yyyy-MM-dd");
                retorno.IdGrupoPermissao = usuario.IdgrupoPermissao;
                retorno.NomeGrupoPermissao = usuario.IdgrupoPermissaoNavigation.Nome;

                return retorno;
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
                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<CadastroUsuarioDTO>(dados);
                var context = new ultimate_mindContext();
                var idEmpresa = 1;

                if (obj.IdUsuario > 0)
                {
                    var usuario = context.Usuario.Where(r => r.Idusuario == obj.IdUsuario).FirstOrDefault();

                    if (usuario == null)
                        return Erro("Usuário não encontrado!!");

                    var isAlteracao = false;

                    if (usuario.Nome != obj.Nome)
                    {
                        usuario.Nome = obj.Nome;
                        isAlteracao = false;
                    }
                    if (usuario.Matricula != obj.Matricula)
                    {
                        usuario.Matricula = obj.Matricula;
                        isAlteracao = true;
                    }
                    if (usuario.Cpf != new Util().RemoveFormatacaoCPF(obj.Cpf))
                    {
                        usuario.Cpf = new Util().RemoveFormatacaoCPF(obj.Cpf);
                        isAlteracao = true;
                    }
                    if (usuario.Telefone != new Util().RemoveFormatacaoTelefone(obj.Telefone))
                    {
                        usuario.Telefone = new Util().RemoveFormatacaoTelefone(obj.Telefone);
                        isAlteracao = true;
                    }
                    if (usuario.Email != obj.Email)
                    {
                        usuario.Email = obj.Email;
                        isAlteracao = true;
                    }
                    if (usuario.Rg != obj.Rg)
                    {
                        usuario.Rg = obj.Rg;
                        isAlteracao = true;
                    }
                    if (usuario.Status != obj.Status)
                    {
                        usuario.Status = obj.Status;
                        isAlteracao = true;
                    }
                    if (usuario.Idcargo != obj.IdCargo)
                    {
                        usuario.Idcargo = obj.IdCargo;
                        isAlteracao = true;
                    }
                    if (usuario.IdgrupoPermissao != obj.IdGrupoPermissao)
                    {
                        usuario.IdgrupoPermissao = obj.IdGrupoPermissao;
                        isAlteracao = true;
                    }
                    if (usuario.DataNascimento != DateTime.Parse(obj.DataNascimento))
                    {
                        usuario.DataNascimento = DateTime.Parse(obj.DataNascimento);
                        isAlteracao = true;
                    }
                    if (usuario.DataAdmissao != DateTime.Parse(obj.DataAdmissao))
                    {
                        usuario.DataAdmissao = DateTime.Parse(obj.DataAdmissao);
                        isAlteracao = true;
                    }
                    if (usuario.DataDemissao != DateTime.Parse(obj.DataDemissao))
                    {
                        usuario.DataDemissao = DateTime.Parse(obj.DataDemissao);
                        isAlteracao = true;
                    }
                    
                    if(isAlteracao)
                    {
                        context.Entry(usuario);
                    }

                }
                else
                {
                    DateTime dataNascimento;
                    var user = new Usuario();

                    user.Idempresa = idEmpresa;
                    user.Nome = obj.Nome;
                    user.Matricula = obj.Matricula;
                    user.Cpf = new Util().RemoveFormatacaoCPF(obj.Cpf);
                    user.Idcargo = obj.IdCargo;
                    user.Email = obj.Email == null ? null : obj.Email;
                    user.DataNascimento =  obj.DataNascimento == null ? null : DateTime.TryParse(obj.DataNascimento, out dataNascimento) ? (DateTime?)dataNascimento : null;
                    user.Status = obj.Status;
                    user.Telefone = obj.Telefone == null ? null : new Util().RemoveFormatacaoTelefone(obj.Telefone);
                    user.Rg = obj.Rg == null ? null : obj.Rg;
                    user.IdgrupoPermissao = obj.IdGrupoPermissao;
                    user.Senha = new Util().GetHashMD5(new Util().RemoveFormatacaoCPF(obj.Cpf).Substring(0, 4));
                    user.DataAdmissao = obj.DataAdmissao == null ? null : DateTime.TryParse(obj.DataAdmissao, out dataNascimento) ? (DateTime?)dataNascimento : null;
                    user.DataDemissao = obj.DataDemissao == null ? null : DateTime.TryParse(obj.DataDemissao, out dataNascimento) ? (DateTime?)dataNascimento : null;

                    context.Usuario.Add(user);
                }

                context.SaveChanges();

                return Ok();

            }
            catch (Exception ex)
            {
                return Erro(ex);
            }
        }

        public object ObterCargos()
        {
            var context = new ultimate_mindContext();
            string q = HttpContext.Request.Query["q"].ToString();

            int idEmpresa = 1;

            if (string.IsNullOrEmpty(q))
            {
                return context.Cargo.Select(r => new
                {
                    r.Idcargo,
                    Nome = r.NomeCargo
                });
            }

            var cargos = context.Cargo.OrderBy(u => u.Idcargo).Select(r => new
            {
                r.Idcargo,
                Nome = r.NomeCargo
            }).ToList();

            return cargos.Where(u => u.Nome.Normalize().Contains(q, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public object ObterStatus()
        {
            return EnumStatusUsuario.ObtenhaLista();
        }

        public object ObterGrupoPermissao()
        {
            var context = new ultimate_mindContext();
            string q = HttpContext.Request.Query["q"].ToString();

            int idEmpresa = 1;

            if (string.IsNullOrEmpty(q))
            {
                return context.GrupoPermissao.Where(r=> r.Idempresa == idEmpresa).Select(r => new
                {
                    r.IdgrupoPermissao,
                    Nome = r.Nome
                });
            }

            var cargos = context.GrupoPermissao.Where(r => r.Idempresa == idEmpresa).OrderBy(u => u.IdgrupoPermissao).Select(r => new
            {
                r.IdgrupoPermissao,
                Nome = r.Nome
            }).ToList();

            return cargos.Where(u => u.Nome.Normalize().Contains(q, StringComparison.OrdinalIgnoreCase)).ToList();

        }

        public object ConsultarEmpresas()
        {
            var context = new ultimate_mindContext();
            try
            {
                var empresas = context.Empresa.Where(r => r.Status == EnumStatusEmpresa.Ativo.ID)
                    .Select(r=> new 
                    {
                        r.Idempresa,
                        r.RazaoSocial,
                        r.NomeFantasia,
                        r.Apelido,
                        Cnpj = r.Cnpj == null ? "" : new Util().FormataCNPJ(r.Cnpj),
                        StatusNome = EnumStatusEmpresa.Obtenha(r.Status),
                        r.Status,
                        r.Endereco,
                        r.Latitude, 
                        r.Longitude,
                        Telefone = r.Telefone == null ? "" : new Util().FormataTelefone(r.Telefone),
                    }).ToList();

                return empresas;
            }
            catch(Exception ex)
            {
                return Erro(ex);
            }
        }
    }
}
