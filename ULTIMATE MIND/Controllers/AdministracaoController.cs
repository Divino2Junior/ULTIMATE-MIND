using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ULTIMATE_MIND.Arquitetura.DTO;
using ULTIMATE_MIND.Arquitetura.Enum;
using ULTIMATE_MIND.Arquitetura.Filtros;
using ULTIMATE_MIND.Arquitetura.Model.UltimateMind;
using ULTIMATE_MIND.Arquitetura.Util;

namespace ULTIMATE_MIND.Controllers
{
    public class AdministracaoController : ControllerPadrao
    {
        public AdministracaoController(IHostingEnvironment hostingEnvironment)
        {
            HostingEnvironment = hostingEnvironment;
        }
        public IActionResult CadastroUsuario()
        {
            return View();
        }
        public IActionResult CadastroGrupoUsuario()
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
                var empresa = GetIDEmpresaLogada();

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
                    return usuarios.OrderBy(r => r.Matricula).ToList();

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
                    .Include(r => r.IdcargoNavigation)
                    .Include(r => r.IdgrupoPermissaoNavigation)
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

                var caminhoFoto = Path.Combine(CaminhoFotoPerfil, $"{usuario.Idusuario}.jpg");

                if (System.IO.File.Exists(caminhoFoto))
                {
                    retorno.ImgUsuario = "/FotoPerfil/" + $"{usuario.Idusuario}.jpg";
                }

                return retorno;
            }
            catch (Exception ex)
            {
                return Erro(ex);
            }
        }
        public IActionResult SalvarUsuario(CadastroUsuarioDTO obj)
        {
            try
            {
                var context = new ultimate_mindContext();
                var idEmpresa = GetIDEmpresaLogada();

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
                    if (obj.Imagem != null && obj.Imagem.Length > 0)
                    {
                        var nomeArquivo = $"{obj.IdUsuario}.jpg";
                        var caminhoCompleto = this.CaminhoFotoPerfil + nomeArquivo;

                        // Verifique se o arquivo já existe
                        if (System.IO.File.Exists(caminhoCompleto))
                        {
                            // Se o arquivo existir, exclua-o antes de salvar o novo
                            System.IO.File.Delete(caminhoCompleto);
                        }

                        using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                        {
                            obj.Imagem.CopyTo(stream);
                        }
                    }

                    if (isAlteracao)
                    {
                        context.Entry(usuario);
                        context.SaveChanges();
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
                    user.DataNascimento = obj.DataNascimento == null ? null : DateTime.TryParse(obj.DataNascimento, out dataNascimento) ? (DateTime?)dataNascimento : null;
                    user.Status = obj.Status;
                    user.Telefone = obj.Telefone == null ? null : new Util().RemoveFormatacaoTelefone(obj.Telefone);
                    user.Rg = obj.Rg == null ? null : obj.Rg;
                    user.IdgrupoPermissao = obj.IdGrupoPermissao;
                    user.Senha = new Util().GetHashMD5(new Util().RemoveFormatacaoCPF(obj.Cpf).Substring(0, 4));
                    user.DataAdmissao = obj.DataAdmissao == null ? null : DateTime.TryParse(obj.DataAdmissao, out dataNascimento) ? (DateTime?)dataNascimento : null;
                    user.DataDemissao = obj.DataDemissao == null ? null : DateTime.TryParse(obj.DataDemissao, out dataNascimento) ? (DateTime?)dataNascimento : null;

                    context.Usuario.Add(user);
                    context.SaveChanges();

                    if (obj.Imagem != null && obj.Imagem.Length > 0)
                    {
                        var nomeArquivo = $"{user.Idusuario}.jpg";
                        var caminhoCompleto = this.CaminhoFotoPerfil + nomeArquivo;

                        // Verifique se o arquivo já existe
                        if (System.IO.File.Exists(caminhoCompleto))
                        {
                            // Se o arquivo existir, exclua-o antes de salvar o novo
                            System.IO.File.Delete(caminhoCompleto);
                        }

                        using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                        {
                            obj.Imagem.CopyTo(stream);
                        }
                    }
                }
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

            int idEmpresa = GetIDEmpresaLogada();

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

            int idEmpresa = GetIDEmpresaLogada();

            if (string.IsNullOrEmpty(q))
            {
                return context.GrupoPermissao.Where(r => r.Idempresa == idEmpresa).Select(r => new
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
                    .Select(r => new
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
            catch (Exception ex)
            {
                return Erro(ex);
            }
        }

        public object BuscarGrupoUsuarios()
        {
            try
            {
                var context = new ultimate_mindContext();
                var idempresa = GetIDEmpresaLogada();

                var gruposUsuario = context.GrupoPermissao.Where(r => r.Idempresa == idempresa)
                    .Select(r => new
                    {
                        r.IdgrupoPermissao,
                        r.Nome,
                    }).ToList();

                return gruposUsuario;
            }
            catch (Exception ex)
            {
                return Erro(ex);
            }
        }

        public object BuscarInfoGrupoUsuario(int id)
        {
            try
            {
                var context = new ultimate_mindContext();

                var lstTelas = context.Tela.Include(r => r.IdgrupoPermissaoNavigation)
                    .Where(r => r.IdgrupoPermissao == id).ToList();

                var retorno = new GrupoUsuarioDTO();

                if (lstTelas.Count > 0)
                {
                    retorno.IdgrupoPermissao = lstTelas.Select(r => r.IdgrupoPermissao).FirstOrDefault();
                    retorno.NomeGrupoUsuario = lstTelas.Select(r => r.IdgrupoPermissaoNavigation.Nome).FirstOrDefault();
                    retorno.lstNomeTela = lstTelas.Select(r => r.NomeTela).ToList();

                    return retorno;
                }
                else
                {
                    var grupoUsuario = context.GrupoPermissao.Where(r => r.IdgrupoPermissao == id).FirstOrDefault();
                    retorno.IdgrupoPermissao = grupoUsuario.IdgrupoPermissao;
                    retorno.NomeGrupoUsuario = grupoUsuario.Nome;
                    return retorno;
                }
            }
            catch (Exception ex)
            {
                return Erro(ex);
            }
        }

        public object SalvarPermissaoGrupoUsuario(string dados)
        {
            try
            {
                var context = new ultimate_mindContext();
                var idempresa = GetIDEmpresaLogada();
                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<FiltroSalvarGrupoUsuario>(dados);

                if (obj.ListaTela.Count > 0)
                {
                    var grupoUser = context.GrupoPermissao.Where(r => r.IdgrupoPermissao == obj.IDGrupoPermissao).FirstOrDefault();
                    if (grupoUser == null)
                    {
                        var grupoPermissao = new GrupoPermissao();
                        grupoPermissao.Nome = obj.NomeGrupoUsuario;
                        grupoPermissao.Idempresa = idempresa;

                        context.GrupoPermissao.Add(grupoPermissao);
                        context.SaveChanges();

                        var lstTelas = new List<Tela>();

                        foreach (var item in obj.ListaTela)
                        {
                            var tela = new Tela();
                            tela.IdgrupoPermissao = grupoPermissao.IdgrupoPermissao;
                            tela.NomeTela = item;
                            lstTelas.Add(tela);
                        }

                        context.Tela.AddRange(lstTelas);
                        context.SaveChanges();
                    }
                    else
                    {
                        if (grupoUser.Nome != obj.NomeGrupoUsuario)
                        {
                            grupoUser.Nome = obj.NomeGrupoUsuario;
                            context.Entry(grupoUser);
                            context.SaveChanges();
                        }

                        var gTelas = context.Tela.Where(r => r.IdgrupoPermissao == obj.IDGrupoPermissao).ToList();

                        if (gTelas.Count > 0)
                        {
                            context.Tela.RemoveRange(gTelas);
                            context.SaveChanges();

                            var lstTelas = new List<Tela>();

                            foreach (var item in obj.ListaTela)
                            {
                                var tela = new Tela();
                                tela.IdgrupoPermissao = obj.IDGrupoPermissao;
                                tela.NomeTela = item;
                                lstTelas.Add(tela);
                            }

                            context.Tela.AddRange(lstTelas);
                            context.SaveChanges();
                        }
                        else
                        {
                            var lstTelas = new List<Tela>();

                            foreach (var item in obj.ListaTela)
                            {
                                var tela = new Tela();
                                tela.IdgrupoPermissao = obj.IDGrupoPermissao;
                                tela.NomeTela = item;
                                lstTelas.Add(tela);
                            }

                            context.Tela.AddRange(lstTelas);
                            context.SaveChanges();
                        }

                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return Erro(ex);
            }
        }
    }
}
