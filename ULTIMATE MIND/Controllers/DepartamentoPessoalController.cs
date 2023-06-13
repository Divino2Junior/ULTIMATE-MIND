using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using ULTIMATE_MIND.Arquitetura.DTO;
using ULTIMATE_MIND.Arquitetura.Enum;
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

        public IActionResult InserirContraCheque(IFormFile arquivoPdf, int usuario, DateTime mesReferencia)
        {
            try
            {

                var context = new ultimate_mindContext();
                var idEmpresa = GetIDEmpresaLogada();
                if (arquivoPdf != null && arquivoPdf.Length > 0)
                {
                    var validacaoContraCheque = context.ValidacaoContraCheque
                        .Where(r => r.Idusuario == usuario && r.Referencia == mesReferencia).FirstOrDefault();

                    if (validacaoContraCheque == null)
                    {
                        // Monta o nome do arquivo usando as informações do usuário e referência do mês
                        var nomeArquivo = $"ContraCheque_{usuario}_{mesReferencia.ToString("dd_MM_yyyy")}.pdf";

                        // Monta o caminho completo do arquivo PDF
                        var caminhoCompleto = this.CaminhoContraCheque + nomeArquivo;

                        // Salva o arquivo PDF no caminho especificado
                        using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                        {
                            arquivoPdf.CopyTo(stream);
                        }

                        var contraCheque = new ValidacaoContraCheque();
                        contraCheque.Idusuario = usuario;
                        contraCheque.DataInserido = DateTime.Now;
                        contraCheque.Referencia = mesReferencia;
                        contraCheque.NomeArquivo = nomeArquivo;
                        contraCheque.Idempresa = idEmpresa;


                        context.ValidacaoContraCheque.Add(contraCheque);
                        context.SaveChanges();
                    }

                    else
                        return Erro("Contra Cheque já cadastrado!!!");


                    return Ok();
                }
                else
                {
                    return BadRequest("Nenhum arquivo PDF foi enviado.");
                }
            }
            catch (Exception ex)
            {
                // Em caso de erro, retorne uma mensagem de erro
                return BadRequest("Ocorreu um erro ao inserir o Contra-Cheque: " + ex.Message);
            }
        }


        public object BuscarContraCheque()
        {
            try
            {
                var context = new ultimate_mindContext();
                var idEmpresa = GetIDEmpresaLogada();

                var contrasCheques = context.ValidacaoContraCheque
                    .Include(r => r.IdusuarioNavigation)
                    .Where(r => r.Idempresa == idEmpresa).ToList();

                var retorno = new List<ContraChequeDTO>();

                foreach (var item in  contrasCheques)
                {

                    string mesFormatado = item.Referencia.ToString("MMMM", new CultureInfo("pt-BR"));
                    string anoFormatado = item.Referencia.ToString("yyyy");

                    //// Formata o mês abreviado
                    //string mesAbreviado = mesReferencia.ToString("MMM", new CultureInfo("pt-BR"));

                    string dataFormatada = $"{mesFormatado}/{anoFormatado}";

                    var contraCheque = new ContraChequeDTO();
                    contraCheque.Matricula = item.IdusuarioNavigation.Matricula;
                    contraCheque.IdContraCheque = item.IdvalidacaoContraCheque;
                    contraCheque.NomeColaborador = item.IdusuarioNavigation.Nome;
                    contraCheque.Referencia = dataFormatada;
                    contraCheque.IsAssinado = item.IsAssinado;
                    retorno.Add(contraCheque);
                }


                if (retorno.Count > 0)
                    return retorno;
                else
                    return null;
            }
            catch (Exception ex)
            {
                return Erro(ex);
            }
        }
    }
}
