using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Net;
using ULTIMATE_MIND.Arquitetura.DTO;
using ULTIMATE_MIND.Arquitetura.Enum;
using ULTIMATE_MIND.Arquitetura.Model.UltimateMind;
using ULTIMATE_MIND.Arquitetura.Util;

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

                var caminhoFoto = Path.Combine(CaminhoQrCodeCliente, $"{cliente.Idcliente}.jpg");

                if (System.IO.File.Exists(caminhoFoto))
                {
                    retorno.UrlFoto = "/QrCodeCliente/" + $"{cliente.Idcliente}.jpg";
                }

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
                    if (clienteExistente.Cpf != cliente.Cpf)
                    {
                        clienteExistente.Cpf = new Util().RemoveFormatacaoCPF(cliente.Cpf);
                        isAlteracao = true;
                    }
                    if (clienteExistente.Cnpj != cliente.Cpnj)
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
                    if (clienteExistente.Latitude != new Util().GetCoordenada(cliente.Latitude))
                    {
                        clienteExistente.Latitude = new Util().GetCoordenada(cliente.Latitude);
                        isAlteracao = true;
                    }
                    if (clienteExistente.Longitude != new Util().GetCoordenada(cliente.Longitude))
                    {
                        clienteExistente.Longitude = new Util().GetCoordenada(cliente.Longitude);
                        isAlteracao = true;
                    }
                    if (clienteExistente.Telefone != new Util().RemoveFormatacaoTelefone(cliente.Telefone))
                    {
                        clienteExistente.Telefone = new Util().RemoveFormatacaoTelefone(cliente.Telefone);
                        isAlteracao = true;
                    }
                    if (clienteExistente.Email != cliente.Email)
                    {
                        clienteExistente.Email = cliente.Email;
                        isAlteracao = true;
                    }
                    if (cliente.Imagem != null && cliente.Imagem.Length > 0)
                    {
                        var nomeArquivo = $"{cliente.idcliente}.jpg";
                        var caminhoCompleto = this.CaminhoQrCodeCliente + nomeArquivo;

                        // Verifique se o arquivo já existe
                        if (System.IO.File.Exists(caminhoCompleto))
                        {
                            // Se o arquivo existir, exclua-o antes de salvar o novo
                            System.IO.File.Delete(caminhoCompleto);
                        }

                        using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                        {
                            cliente.Imagem.CopyTo(stream);
                        }
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
                        Cpf = new Util().RemoveFormatacaoCPF(cliente.Cpf),
                        Cnpj = new Util().RemoveFormatacaoCNPJ(cliente.Cpnj),
                        Status = cliente.Status,
                        Endereco = cliente.Endereco,
                        Latitude = new Util().GetCoordenada(cliente.Latitude),
                        Longitude = new Util().GetCoordenada(cliente.Longitude),
                        Telefone = new Util().RemoveFormatacaoTelefone(cliente.Telefone),
                        Email = cliente.Email
                    };

                    context.Cliente.Add(novoCliente);
                    context.SaveChanges();

                    if (cliente.Imagem != null && cliente.Imagem.Length > 0)
                    {
                        var nomeArquivo = $"{novoCliente.Idcliente}.jpg";
                        var caminhoCompleto = this.CaminhoQrCodeCliente + nomeArquivo;

                        // Verifique se o arquivo já existe
                        if (System.IO.File.Exists(caminhoCompleto))
                        {
                            // Se o arquivo existir, exclua-o antes de salvar o novo
                            System.IO.File.Delete(caminhoCompleto);
                        }

                        using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                        {
                            cliente.Imagem.CopyTo(stream);
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
                var cliente = context.Cliente.Where(r => r.Idcliente == id).FirstOrDefault();

                if (cliente == null)
                    throw new Exception("Cliente não cadastrado");


                var atendimento = context.Atendimento.Where(r => r.Idusuario == user.IDUsuario
                && r.Idcliente == id && r.DataAtendimento == DateTime.Now.Date && r.FimAtendimento == null).FirstOrDefault();

                var novoAtendimento = new Atendimento();
                if (atendimento == null)
                {
                    novoAtendimento.Idusuario = user.IDUsuario;
                    novoAtendimento.Idcliente = cliente.Idcliente;
                    novoAtendimento.DataAtendimento = DateTime.Now.Date;
                    novoAtendimento.InicioAtendimento = DateTime.Now;
                    novoAtendimento.InicioAtendimentoLat = latitude;
                    novoAtendimento.InicioAtendimentoLong = longitude;

                    context.Add(novoAtendimento);
                    context.SaveChanges();
                }
                else
                    throw new Exception("Já existe atendimento hoje para este cliente");

                var caminhoFoto = Path.Combine(CaminhoQrCodeCliente, $"{cliente.Idcliente}.jpg");

                if (System.IO.File.Exists(caminhoFoto))
                {
                    var IdAtendimento = novoAtendimento == null ? atendimento.Idatendimento : novoAtendimento.Idatendimento;
                    var urlFoto = "/QrCodeCliente/" + $"{cliente.Idcliente}.jpg";
                    return new { urlFoto, IdAtendimento };
                }

                return null;
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

                var atendimento = context.Atendimento
                    .Include(r => r.IdclienteNavigation)
                    .Where(r => r.Idusuario == user.IDUsuario && r.IdclienteNavigation.Idempresa == idempresa && r.DataAtendimento == DateTime.Now.Date)
                    .Select(r => new
                    {
                        r.Idatendimento,
                        r.Idcliente,
                        Nome = r.IdclienteNavigation.NomeCliente,
                        Data = r.DataAtendimento.ToString("dd/MM/yyyy"),
                        Status = r.FimAtendimento == null ? "Pendente" : "Finalizado"

                    }).OrderBy(r => r.Status != "Pendente").ToList();

                return atendimento;
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
                    .Include(r=> r.IdclienteNavigation).Where(r => r.IdclienteNavigation.Idempresa == idEmpresa).OrderBy(r => r.Idcliente)
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

                var obra = context.Obra.Include(r=> r.IdclienteNavigation). Where(r => r.Idobra == id).FirstOrDefault();

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

                var caminhoFoto = Path.Combine(CaminhoQrCodObra, $"{obra.Idobra}.jpg");

                if (System.IO.File.Exists(caminhoFoto))
                {
                    retorno.UrlFoto = "/QrCodeObra/" + $"{obra.Idcliente}.jpg";
                }

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

                    // Verifique se uma imagem foi enviada
                    if (obra.ImagemObra != null && obra.ImagemObra.Length > 0)
                    {
                        var nomeArquivo = $"{novaObra.Idobra}.jpg";
                        var caminhoCompleto = this.CaminhoQrCodObra + nomeArquivo;

                        // Verifique se o arquivo já existe
                        if (System.IO.File.Exists(caminhoCompleto))
                        {
                            // Se o arquivo existir, exclua-o antes de salvar o novo
                            System.IO.File.Delete(caminhoCompleto);
                        }

                        using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                        {
                            obra.ImagemObra.CopyTo(stream);
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
