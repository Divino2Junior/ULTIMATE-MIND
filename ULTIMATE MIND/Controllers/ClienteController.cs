using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using ULTIMATE_MIND.Arquitetura.DTO;
using ULTIMATE_MIND.Arquitetura.Enum;
using ULTIMATE_MIND.Arquitetura.Model.UltimateMind;
using ULTIMATE_MIND.Arquitetura.Util;
using ZXing;
using ZXing.Common;
using ZXing.Presentation;

namespace ULTIMATE_MIND.Controllers
{
    public class ClienteController : ControllerPadrao
    {

        public ClienteController(IHostingEnvironment hostingEnvironment)
        {
            HostingEnvironment = hostingEnvironment;
        }
        public IActionResult CadastroCliente()
        {
            return View();
        }
        public IActionResult Atendimento()
        {
            return View();
        }
        public IActionResult CadastroObra()
        {
            return View();
        }
        public IActionResult CadastroObraUsuario()
        {
            return View();
        }

        public object BuscarClientes()
        {
            try
            {
                var context = new ultimate_mindContext();
                var idEmpresa = GetIDEmpresaLogada();

                var clientes = context.Cliente.Where(r => r.Idempresa == idEmpresa).OrderBy(r => r.Idcliente)
                    .Select(r => new
                    {
                        r.Idcliente,
                        Apelido = r.NomeCliente,
                        CpfouCnpj = r.Cpf == null ? (r.Cnpj == null ? "" : new Util().FormataCNPJ(r.Cnpj)) : new Util().FormataCPF(r.Cpf),
                        Status = EnumStatusCliente.Obtenha(r.Status),
                        Telefone = r.Telefone == null ? "" : new Util().FormataTelefone(r.Telefone)
                    }).ToList();

                return clientes;
            }
            catch (Exception ex)
            {
                return Erro(ex);
            }
        }

        public object BuscarInfoCliente(int id)
        {
            try
            {
                var context = new ultimate_mindContext();

                var cliente = context.Cliente.Where(r => r.Idcliente == id).FirstOrDefault();

                if (cliente == null)
                    return Erro("Cliente não encontrado!!");

                var retorno = new ClienteDTO();

                retorno.idcliente = cliente.Idcliente;
                retorno.Nome = cliente.NomeCliente;
                retorno.Cpf = cliente.Cpf == null ? "" : new Util().FormataCPF(cliente.Cpf);
                retorno.Cpnj = cliente.Cnpj == null ? "" : new Util().FormataCNPJ(cliente.Cnpj);
                retorno.Status = cliente.Status;
                retorno.NomeStatus = EnumStatusCliente.Obtenha(retorno.Status);
                retorno.Endereco = cliente.Endereco;
                retorno.Longitude = cliente.Longitude.ToString();
                retorno.Latitude = cliente.Latitude.ToString();
                retorno.Telefone = cliente.Telefone == null ? "" : new Util().FormataTelefone(cliente.Telefone);
                retorno.Email = cliente.Email ?? "";

                return retorno;

            }
            catch (Exception ex)
            {
                return Erro(ex);
            }
        }

        public IActionResult SalvarCliente(ClienteDTO cliente)
        {
            try
            {
                var context = new ultimate_mindContext();
                var idEmpresa = GetIDEmpresaLogada();

                if (cliente.idcliente > 0)
                {
                    var clienteExistente = context.Cliente.FirstOrDefault(c => c.Idcliente == cliente.idcliente);

                    if (clienteExistente == null)
                        return Erro("Cliente não encontrado!");

                    // Verifica se houve alteração nos campos
                    var isAlteracao = false;

                    if (clienteExistente.NomeCliente != cliente.Nome)
                    {
                        clienteExistente.NomeCliente = cliente.Nome;
                        isAlteracao = true;
                    }
                    if (cliente.Cpf != null && clienteExistente.Cpf != new Util().RemoveFormatacaoCPF(cliente.Cpf))
                    {
                        clienteExistente.Cpf = new Util().RemoveFormatacaoCPF(cliente.Cpf);
                        isAlteracao = true;
                    }
                    if (cliente.Cpnj != null && clienteExistente.Cnpj != new Util().RemoveFormatacaoCNPJ(cliente.Cpnj))
                    {
                        clienteExistente.Cnpj = new Util().RemoveFormatacaoCNPJ(cliente.Cpnj);
                        isAlteracao = true;
                    }
                    if (clienteExistente.Status != cliente.Status)
                    {
                        clienteExistente.Status = cliente.Status;
                        isAlteracao = true;
                    }
                    if (clienteExistente.Endereco != cliente.Endereco)
                    {
                        clienteExistente.Endereco = cliente.Endereco;
                        isAlteracao = true;
                    }
                    if (cliente.Latitude != null && clienteExistente.Latitude != new Util().GetCoordenada(cliente.Latitude))
                    {
                        clienteExistente.Latitude = new Util().GetCoordenada(cliente.Latitude);
                        isAlteracao = true;
                    }
                    if (cliente.Longitude != null && clienteExistente.Longitude != new Util().GetCoordenada(cliente.Longitude))
                    {
                        clienteExistente.Longitude = new Util().GetCoordenada(cliente.Longitude);
                        isAlteracao = true;
                    }
                    if (cliente.Telefone != null && clienteExistente.Telefone != new Util().RemoveFormatacaoTelefone(cliente.Telefone))
                    {
                        clienteExistente.Telefone = new Util().RemoveFormatacaoTelefone(cliente.Telefone);
                        isAlteracao = true;
                    }
                    if (clienteExistente.Email != cliente.Email)
                    {
                        clienteExistente.Email = cliente.Email;
                        isAlteracao = true;
                    }

                    if (isAlteracao)
                    {
                        context.Entry(clienteExistente);
                        context.SaveChanges();
                    }
                }
                else
                {
                    var novoCliente = new Cliente
                    {
                        NomeCliente = cliente.Nome,
                        Idempresa = idEmpresa,
                        Cpf = cliente.Cpf == null ? null : new Util().RemoveFormatacaoCPF(cliente.Cpf),
                        Cnpj = cliente.Cpnj == null ? null : new Util().RemoveFormatacaoCNPJ(cliente.Cpnj),
                        Status = cliente.Status,
                        Endereco = cliente.Endereco,
                        Latitude = cliente.Latitude == null ? 0.0 : new Util().GetCoordenada(cliente.Latitude),
                        Longitude = cliente.Longitude == null ? 0.0 : new Util().GetCoordenada(cliente.Longitude),
                        Telefone = cliente.Telefone == null ? null : new Util().RemoveFormatacaoTelefone(cliente.Telefone),
                        Email = cliente.Email
                    };

                    context.Cliente.Add(novoCliente);
                    context.SaveChanges();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return Erro(ex);
            }
        }

        public object BuscarSelectCliente()
        {
            try
            {
                var context = new ultimate_mindContext();
                string q = HttpContext.Request.Query["q"].ToString();

                int idEmpresa = GetIDEmpresaLogada();

                if (string.IsNullOrEmpty(q))
                {
                    return context.Cliente.Where(r => r.Idempresa == idEmpresa).Select(r => new
                    {
                        r.Idcliente,
                        Nome = r.NomeCliente
                    });
                }

                var clientes = context.Cliente.Where(r => r.Idempresa == idEmpresa).OrderBy(u => u.Idcliente).Select(r => new
                {
                    r.Idcliente,
                    Nome = r.NomeCliente
                }).ToList();

                return clientes.Where(u => u.Nome.Normalize().Contains(q, StringComparison.OrdinalIgnoreCase)).ToList();

            }
            catch (Exception ex)
            {
                return Erro(ex);
            }
        }

        public object BuscarQrCodeCliente(int id, float latitude, float longitude)
        {
            try
            {
                var context = new ultimate_mindContext();
                var user = GetUsuarioLogado();

                var obra = context.ObraUsuario.Where(r => r.Idusuario == user.IDUsuario && r.Idobra == id).FirstOrDefault();

                if (obra == null)
                    throw new Exception("Obra não relacionada ao Usuário");


                var atendimento = context.Atendimento.Where(r => r.Idusuario == user.IDUsuario
                && r.IdobraUsuario == id && r.DataAtendimento == DateTime.Now.Date && r.FimAtendimento == null).FirstOrDefault();

                var novoAtendimento = new Atendimento();
                if (atendimento == null)
                {
                    novoAtendimento.Idusuario = user.IDUsuario;
                    novoAtendimento.IdobraUsuario = obra.IdobraUsuario;
                    novoAtendimento.DataAtendimento = DateTime.Now.Date;
                    novoAtendimento.InicioAtendimento = DateTime.Now;
                    novoAtendimento.InicioAtendimentoLat = latitude;
                    novoAtendimento.InicioAtendimentoLong = longitude;

                    context.Add(novoAtendimento);
                    context.SaveChanges();
                }
                else
                    throw new Exception("Já existe atendimento em aberto para este cliente");

                var caminhoFoto = Path.Combine(CaminhoQrCodObra, $"{obra.Idobra}_{obra.Idusuario}.jpg");

                var IdAtendimento = 0;
                var urlFoto = "";
                if (System.IO.File.Exists(caminhoFoto))
                {
                    IdAtendimento = novoAtendimento == null ? atendimento.Idatendimento : novoAtendimento.Idatendimento;
                    urlFoto = "/QrCodeObra/" + $"{obra.Idobra}_{obra.Idusuario}.jpg";
                    return new { urlFoto, IdAtendimento };
                }

                IdAtendimento = novoAtendimento == null ? atendimento.Idatendimento : novoAtendimento.Idatendimento;
                return new { urlFoto = "", IdAtendimento };
            }
            catch (Exception ex)
            {
                return Erro(ex);
            }
        }

        public object BuscarAtendimentos()
        {
            try
            {
                var context = new ultimate_mindContext();
                var idempresa = GetIDEmpresaLogada();
                var user = GetUsuarioLogado();

                var atendimentos = context.Atendimento
                    .Include(r => r.IdobraUsuarioNavigation).ThenInclude(r => r.IdobraNavigation)
                    .Where(r => r.Idusuario == user.IDUsuario && r.IdobraUsuarioNavigation.IdobraNavigation.IdclienteNavigation.Idempresa == idempresa
                    && r.DataAtendimento.Date == DateTime.Now.Date)
                    .Select(r => new
                    {
                        r.Idatendimento,
                        r.IdobraUsuario,
                        Nome = r.IdobraUsuarioNavigation.IdobraNavigation.NomeObra,
                        Data = r.DataAtendimento.ToString("dd/MM/yyyy"),
                        InicioAtendimento = r.InicioAtendimento.ToString("HH:mm"),
                        FimAtendimento = r.FimAtendimento == null ? "" : r.FimAtendimento.Value.ToString("HH:mm"),
                        Status = "Finalizado"

                    }).ToList();

                var atendimento = context.Atendimento
                    .Include(r => r.IdobraUsuarioNavigation).ThenInclude(r => r.IdobraNavigation)
                    .Where(r => r.Idusuario == user.IDUsuario && r.IdobraUsuarioNavigation.IdobraNavigation.IdclienteNavigation.Idempresa == idempresa
                    && r.DataAtendimento.Date == DateTime.Now.Date && r.FimAtendimento == null)
                    .Select(r => new
                    {
                        r.Idatendimento,
                        r.IdobraUsuario,
                        IdObra = r.IdobraUsuarioNavigation.Idobra,
                        Nome = r.IdobraUsuarioNavigation.IdobraNavigation.NomeObra,
                        Data = r.DataAtendimento.ToString("dd/MM/yyyy"),
                        InicioAtendimento = r.InicioAtendimento.ToString("HH:mm")
                    }).FirstOrDefault();

                return new { atendimento, atendimentos };
            }
            catch (Exception ex)
            {
                return Erro(ex);
            }
        }

        public object FinalizarAtendimento(int id, float latitude, float longitude, string observacao)
        {
            try
            {
                var context = new ultimate_mindContext();
                var user = GetUsuarioLogado();

                var atendimento = context.Atendimento.Where(r => r.Idatendimento == id).FirstOrDefault();

                if (atendimento == null)
                    throw new Exception("Atendimento não encontrado");
                else
                {
                    string observacaoDecodificada = WebUtility.UrlDecode(observacao);

                    atendimento.FimAtendimento = DateTime.Now;
                    atendimento.FimAtendimentoLat = latitude;
                    atendimento.FimAtendimentoLong = longitude;
                    atendimento.Observacao = observacaoDecodificada;
                    context.Entry(atendimento);
                    context.SaveChanges();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return Erro(ex);
            }
        }

        public object BuscarObras()
        {
            try
            {
                var context = new ultimate_mindContext();
                var idEmpresa = GetIDEmpresaLogada();

                var obras = context.Obra
                    .Include(r => r.IdclienteNavigation).Where(r => r.IdclienteNavigation.Idempresa == idEmpresa).OrderBy(r => r.Idcliente)
                    .Select(r => new
                    {
                        r.Idobra,
                        NomeCliente = r.IdclienteNavigation.NomeCliente,
                        Status = EnumStatusObra.Obtenha(r.Status),
                        r.Endereco
                    }).ToList();

                return obras;
            }
            catch (Exception ex)
            {
                return Erro(ex);
            }
        }

        public object BuscarInfoObra(int id)
        {
            try
            {
                var context = new ultimate_mindContext();

                var obra = context.Obra.Include(r => r.IdclienteNavigation).Where(r => r.Idobra == id).FirstOrDefault();

                if (obra == null)
                    return Erro("Cliente não encontrado!!");

                var retorno = new ObraDTO();

                retorno.Idobra = obra.Idobra;
                retorno.Idcliente = obra.Idcliente;
                retorno.NomeCliente = obra.IdclienteNavigation.NomeCliente;
                retorno.Status = obra.Status;
                retorno.NomeStatus = EnumStatusCliente.Obtenha(retorno.Status);
                retorno.Endereco = obra.Endereco;
                retorno.Longitude = obra.Longitude.ToString();
                retorno.Latitude = obra.Latitude.ToString();
                retorno.NomeObra = obra.NomeObra;

                return retorno;

            }
            catch (Exception ex)
            {
                return Erro(ex);
            }
        }

        public IActionResult SalvarObra(ObraDTO obra)
        {
            try
            {
                var context = new ultimate_mindContext();
                var idEmpresa = GetIDEmpresaLogada();

                if (obra.Idobra > 0)
                {
                    var obraExistente = context.Obra.FirstOrDefault(o => o.Idobra == obra.Idobra);

                    if (obraExistente == null)
                        return Erro("Obra não encontrada!");

                    // Verifique se houve alteração nos campos
                    var isAlteracao = false;

                    if (obraExistente.NomeObra != obra.NomeObra)
                    {
                        obraExistente.NomeObra = obra.NomeObra;
                        isAlteracao = true;
                    }
                    if (obraExistente.Endereco != obra.Endereco)
                    {
                        obraExistente.Endereco = obra.Endereco;
                        isAlteracao = true;
                    }
                    if (obraExistente.Status != obra.Status)
                    {
                        obraExistente.Status = obra.Status;
                        isAlteracao = true;
                    }
                    if (obraExistente.Latitude != new Util().GetCoordenada(obra.Latitude))
                    {
                        obraExistente.Latitude = new Util().GetCoordenada(obra.Latitude);
                        isAlteracao = true;
                    }
                    if (obraExistente.Longitude != new Util().GetCoordenada(obra.Longitude))
                    {
                        obraExistente.Longitude = new Util().GetCoordenada(obra.Longitude);
                        isAlteracao = true;
                    }

                    if (isAlteracao)
                    {
                        context.SaveChanges();
                    }
                }
                else
                {
                    var novaObra = new Obra
                    {
                        Idcliente = obra.Idcliente,
                        NomeObra = obra.NomeObra,
                        Endereco = obra.Endereco,
                        Status = obra.Status,
                        Latitude = new Util().GetCoordenada(obra.Latitude),
                        Longitude = new Util().GetCoordenada(obra.Longitude)
                    };

                    context.Obra.Add(novaObra);
                    context.SaveChanges();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return Erro(ex);
            }
        }

        public object BuscarRelacionamentoObraUsuario()
        {
            try
            {
                var context = new ultimate_mindContext();
                var idEmpresa = GetIDEmpresaLogada();

                var obrasUsuario = context.ObraUsuario
                    .Include(r => r.IdobraNavigation)
                    .Include(r => r.IdusuarioNavigation).Where(r => r.IdobraNavigation.IdclienteNavigation.Idempresa == idEmpresa)
                    .Select(r => new
                    {
                        r.IdobraUsuario,
                        NomeObra = r.IdobraNavigation.NomeObra,
                        NomeUsuario = r.IdusuarioNavigation.Nome,
                        Status = EnumStatusObra.Obtenha(r.IdobraNavigation.Status),
                    }).ToList();

                return obrasUsuario;
            }
            catch (Exception ex)
            {
                return Erro(ex);
            }
        }
        public object BuscarInfoRelacionamentoObraUsuario(int id)
        {
            try
            {
                var context = new ultimate_mindContext();

                var relacionamento = context.ObraUsuario
                    .Include(r => r.IdobraNavigation)
                    .Include(r => r.IdusuarioNavigation)
                    .Where(r => r.IdobraUsuario == id)
                    .FirstOrDefault();

                if (relacionamento == null)
                    return Erro("Relacionamento obra-usuário não encontrado!");

                var retorno = new RelacionamentoObraUsuarioDTO
                {
                    IdRelacionamentoObraUsuario = relacionamento.IdobraUsuario,
                    IdObra = relacionamento.Idobra,
                    NomeObra = relacionamento.IdobraNavigation.NomeObra,
                    IdUsuario = relacionamento.Idusuario,
                    NomeUsuario = relacionamento.IdusuarioNavigation.Nome,
                    Status = relacionamento.IdobraNavigation.Status
                };

                var caminhoFoto = Path.Combine(CaminhoQrCodObra, $"{relacionamento.Idobra}_{relacionamento.Idusuario}.jpg");

                if (System.IO.File.Exists(caminhoFoto))
                {
                    retorno.ImageUrl = "/QrCodeObra/" + $"{relacionamento.Idobra}_{relacionamento.Idusuario}.jpg";
                }

                return retorno;
            }
            catch (Exception ex)
            {
                return Erro(ex);
            }
        }
        public IActionResult SalvarRelacionamentoObraUsuario(RelacionamentoObraUsuarioDTO relacionamentoObraUsuario)
        {
            try
            {
                var context = new ultimate_mindContext();
                var idEmpresa = GetIDEmpresaLogada();

                if (relacionamentoObraUsuario.IdRelacionamentoObraUsuario > 0)
                {
                    var relacionamentoExistente = context.ObraUsuario
                        .FirstOrDefault(r => r.IdobraUsuario == relacionamentoObraUsuario.IdRelacionamentoObraUsuario);

                    if (relacionamentoExistente == null)
                        return Erro("Relacionamento obra-usuário não encontrado!");

                    // Verifique se houve alteração nos campos
                    var isAlteracao = false;

                    if (relacionamentoExistente.Idobra != relacionamentoObraUsuario.IdObra)
                    {
                        relacionamentoExistente.Idobra = relacionamentoObraUsuario.IdObra;
                        isAlteracao = true;
                    }
                    if (relacionamentoExistente.Idusuario != relacionamentoObraUsuario.IdUsuario)
                    {
                        relacionamentoExistente.Idusuario = relacionamentoObraUsuario.IdUsuario;
                        isAlteracao = true;
                    }

                    if (relacionamentoObraUsuario.Foto != null && relacionamentoObraUsuario.Foto.Length > 0)
                    {
                        var nomeArquivo = $"{relacionamentoObraUsuario.IdObra}_{relacionamentoObraUsuario.IdUsuario}.jpg";
                        var caminhoCompleto = this.CaminhoQrCodObra + nomeArquivo;

                        var caminhoteste = HostingEnvironment.WebRootPath + "\\QrCodeObra\\" + $"{relacionamentoExistente.Idobra}_{relacionamentoExistente.Idusuario}.jpg";

                        // Verifique se o arquivo já existe
                        if (System.IO.File.Exists(caminhoCompleto))
                        {
                            // Se o arquivo existir, exclua-o antes de salvar o novo
                            System.IO.File.Delete(caminhoCompleto);
                        }

                        if (relacionamentoObraUsuario.Foto != null)
                        {
                            using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                            {
                                relacionamentoObraUsuario.Foto.CopyTo(stream);
                            }
                        }
                    }

                    if (isAlteracao)
                    {
                        context.Entry(relacionamentoExistente);
                        context.SaveChanges();
                    }
                }
                else
                {
                    var novoRelacionamento = new ObraUsuario
                    {
                        Idobra = relacionamentoObraUsuario.IdObra,
                        Idusuario = relacionamentoObraUsuario.IdUsuario,
                    };

                    context.ObraUsuario.Add(novoRelacionamento);
                    context.SaveChanges();

                    var nomeArquivo = $"{relacionamentoObraUsuario.IdObra}_{relacionamentoObraUsuario.IdUsuario}.jpg";
                    var caminhoCompleto = this.CaminhoQrCodObra + nomeArquivo;

                    // Verifique se o arquivo já existe
                    if (System.IO.File.Exists(caminhoCompleto))
                    {
                        // Se o arquivo existir, exclua-o antes de salvar o novo
                        System.IO.File.Delete(caminhoCompleto);
                    }

                    if (relacionamentoObraUsuario.Foto != null)
                    {
                        using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                        {
                            relacionamentoObraUsuario.Foto.CopyTo(stream);
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

        public object BuscarSelectObra()
        {
            try
            {
                var context = new ultimate_mindContext();
                string q = HttpContext.Request.Query["q"].ToString();

                int idEmpresa = GetIDEmpresaLogada();

                if (string.IsNullOrEmpty(q))
                {
                    return context.Obra.Where(r => r.IdclienteNavigation.Idempresa == idEmpresa).OrderBy(u => u.Idcliente).Select(r => new
                    {
                        r.Idobra,
                        Nome = r.NomeObra
                    });
                }

                var clientes = context.Obra.Where(r => r.IdclienteNavigation.Idempresa == idEmpresa).OrderBy(u => u.Idcliente).Select(r => new
                {
                    r.Idobra,
                    Nome = r.NomeObra
                });

                return clientes.Where(u => u.Nome.Normalize().Contains(q, StringComparison.OrdinalIgnoreCase)).ToList();

            }
            catch (Exception ex)
            {
                return Erro(ex);
            }
        }
    }
}
