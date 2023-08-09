using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
        public DepartamentoPessoalController(IHostingEnvironment hostingEnvironment)
        {
            HostingEnvironment = hostingEnvironment;
        }

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

        public IActionResult PontoEletronico()
        {
            return View();
        }
        public IActionResult LiberacaoPonto()
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
                    return context.Usuario.Where(r => r.Idempresa == idEmpresa).Select(r => new
                    {
                        r.Idusuario,
                        Nome = r.Matricula + " - " + r.Nome
                    }).ToList();
                }

                var usuarios = context.Usuario.Where(r => r.Idempresa == idEmpresa).OrderBy(u => Convert.ToInt32(u.Matricula)).Select(r => new
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
                    .Where(r => r.Idempresa == idEmpresa && r.IsAssinado != true).ToList();

                var retorno = new List<ContraChequeDTO>();

                foreach (var item in contrasCheques)
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
                    contraCheque.Referencia = dataFormatada.ToUpper();
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

        public object ExcluirContraChequeUsuario(int id)
        {
            try
            {
                var context = new ultimate_mindContext();
                var idempresa = GetIDEmpresaLogada();

                var contraCheque = context.ValidacaoContraCheque.Where(r => r.IdvalidacaoContraCheque == id).FirstOrDefault();

                if (contraCheque == null)
                    return Erro("Contra Cheque não encontrado!");

                var nomeArquivo = $"ContraCheque_{contraCheque.Idusuario}_{contraCheque.Referencia.ToString("dd_MM_yyyy")}.pdf";
                var caminhoCompleto = this.CaminhoContraCheque + nomeArquivo;

                // Verifique se o arquivo já existe
                if (System.IO.File.Exists(caminhoCompleto))
                {
                    // Se o arquivo existir, exclua-o antes de salvar o novo
                    System.IO.File.Delete(caminhoCompleto);
                }

                context.ValidacaoContraCheque.Remove(contraCheque);
                context.SaveChanges();

                return Ok();


            }
            catch (Exception ex)
            {
                return Erro(ex);
            }

        }

        public object ConsultarValidacaoContraCheque()
        {
            try
            {
                var context = new ultimate_mindContext();
                var idEmpresa = GetIDEmpresaLogada();
                var user = GetUsuarioLogado();

                var contrasCheques = context.ValidacaoContraCheque
                    .Include(r => r.IdusuarioNavigation)
                    .Where(r => r.Idempresa == idEmpresa && r.Idusuario == user.IDUsuario).ToList();

                var retorno = new List<ContraChequeDTO>();

                foreach (var item in contrasCheques)
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
                    contraCheque.MesReferencia = mesFormatado.ToUpper();
                    contraCheque.AnoReferencia = anoFormatado.ToUpper();
                    contraCheque.IsAssinado = item.IsAssinado;

                    var nomeArquivo = $"ContraCheque_{item.Idusuario}_{item.Referencia.ToString("dd_MM_yyyy")}.pdf";
                    var caminhoCompleto = this.CaminhoContraCheque + nomeArquivo;

                    // Verifique se o arquivo já existe
                    if (System.IO.File.Exists(caminhoCompleto))
                    {
                        contraCheque.UrlPdf = "/ContraCheques/" + nomeArquivo;
                    }

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

        public object ConsultarPontoDia()
        {
            try
            {
                var context = new ultimate_mindContext();
                var usuario = GetUsuarioLogado();
                var ponto = context.Ponto.Where(r => r.DataPonto.Date == DateTime.Now.Date && r.Idusuario == usuario.IDUsuario).FirstOrDefault();

                var retorno = new PontoMarcadoDTO();

                retorno.DataAtual = DateTime.Now.ToString("dd/MM/yyyy");
                if (ponto != null)
                {
                    retorno.InicioDia = ponto.InicioDia == null ? null : ponto.InicioDia.Value.ToString("HH:mm");
                    retorno.InicioAlmoco = ponto.InicioAlmoco == null ? null : ponto.InicioAlmoco.Value.ToString("HH:mm");
                    retorno.FimAlmoco = ponto.FimAlmoco == null ? null : ponto.FimAlmoco.Value.ToString("HH:mm");
                    retorno.FimDia = ponto.FimDia == null ? null : ponto.FimDia.Value.ToString("HH:mm");
                }
                else
                {
                    retorno.InicioDia = null;
                    retorno.InicioAlmoco = null;
                    retorno.FimAlmoco = null;
                    retorno.FimDia = null;
                }

                return retorno;
            }
            catch (Exception ex)
            {
                return Erro(ex);
            }

        }
        public object SalvarMarcacaoPonto(string dados)
        {
            try
            {
                var context = new ultimate_mindContext();
                var idempresa = GetIDEmpresaLogada();
                var idUsuario = GetUsuarioLogado();
                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<FiltroSalvarMarcacaoPonto>(dados);

                if (idUsuario == null)
                {
                    return Erro("Usuário não esta logado, por favor logue novamente!!");
                }
                var usuario = context.Usuario.Where(r => r.Idusuario == idUsuario.IDUsuario).FirstOrDefault();
                var ponto = context.Ponto.Where(r => r.DataPonto.Date == DateTime.Now.Date && r.Idusuario == usuario.Idusuario).FirstOrDefault();

                if (usuario.IsLiberacaoPonto == true)
                {
                    if (ponto == null)
                    {
                        var newPonto = new Ponto();
                        if (obj.TipoMarcacao == EnumTipoMarcacao.InicioDia.ID)
                        {
                            newPonto.DataPonto = DateTime.Now.Date;
                            newPonto.Idusuario = usuario.Idusuario;
                            newPonto.InicioDia = DateTime.Now;
                            newPonto.InicioDiaLat = new Util().GetCoordenada(obj.Latitude);
                            newPonto.InicioDiaLong = new Util().GetCoordenada(obj.Longitude);
                        }
                        else if (obj.TipoMarcacao == EnumTipoMarcacao.InicioAlmoco.ID)
                        {
                            return Erro("Para iniciar o almoço é necessário iniciar o dia!");
                        }
                        else if (obj.TipoMarcacao == EnumTipoMarcacao.FimAlmoco.ID)
                        {
                            return Erro("Para finalizar o almoço é necessário iniciar o mesmo!");
                        }
                        else if (obj.TipoMarcacao == EnumTipoMarcacao.FimDia.ID)
                        {
                            return Erro("Para finalizar o dia é necessário iniciar o mesmo!");
                        }

                        context.Ponto.Add(newPonto);


                        usuario.IsLiberacaoPonto = false;
                        context.Entry(usuario);
                        context.SaveChanges();
                    }
                    else
                    {
                        if (ponto.InicioDia == null && obj.TipoMarcacao == EnumTipoMarcacao.InicioDia.ID)
                        {
                            ponto.InicioDia = DateTime.Now;
                            ponto.InicioDiaLat = new Util().GetCoordenada(obj.Latitude);
                            ponto.InicioDiaLong = new Util().GetCoordenada(obj.Longitude);
                        }
                        else if (ponto.InicioAlmoco == null && obj.TipoMarcacao == EnumTipoMarcacao.InicioAlmoco.ID)
                        {
                            ponto.InicioAlmoco = DateTime.Now;
                            ponto.InicioAlmocoLat = new Util().GetCoordenada(obj.Latitude);
                            ponto.InicioAlmocoLong = new Util().GetCoordenada(obj.Longitude);
                        }
                        else if (ponto.FimAlmoco == null && obj.TipoMarcacao == EnumTipoMarcacao.FimAlmoco.ID)
                        {
                            if (ponto.InicioAlmoco == null)
                            {
                                return Erro("Para finalizar o almoço é necessário iniciar o mesmo!");
                            }

                            ponto.FimAlmoco = DateTime.Now;
                            ponto.FimAlmocoLat = new Util().GetCoordenada(obj.Latitude);
                            ponto.FimAlmocoLong = new Util().GetCoordenada(obj.Longitude);
                        }
                        else if (ponto.FimDia == null && obj.TipoMarcacao == EnumTipoMarcacao.FimDia.ID)
                        {
                            if (ponto.InicioDia == null)
                            {
                                return Erro("Para finalizar o dia é necessário iniciar o mesmo!");
                            }

                            ponto.FimDia = DateTime.Now;
                            ponto.FimDiaLat = new Util().GetCoordenada(obj.Latitude);
                            ponto.FimDiaLong = new Util().GetCoordenada(obj.Longitude);
                        }

                        usuario.IsLiberacaoPonto = false;
                        context.Entry(usuario);
                        context.SaveChanges();
                    }
                }
                else
                {
                    // Suponha que a string de hora de entrada seja "10:30" e a string de hora de saída seja "17:45"
                    string horaEntradaStr = usuario.HoraEntrada;
                    string horaSaidaStr = usuario.HoraSaida;
                    string horaInicioAlmocoStr = usuario.HoraInicioAlmoco;
                    string horaFimAlmocoStr = usuario.HoraFimAlmoco;

                    // Defina o formato da string de hora
                    string formatoHora = "HH:mm";

                    var horaAtual = DateTime.Now;

                    // Converta as strings para objetos DateTime
                    DateTime horaEntrada = DateTime.ParseExact(horaEntradaStr, formatoHora, CultureInfo.InvariantCulture);
                    DateTime horaSaida = DateTime.ParseExact(horaSaidaStr, formatoHora, CultureInfo.InvariantCulture);
                    DateTime horaInicioAlmoco = DateTime.ParseExact(horaInicioAlmocoStr, formatoHora, CultureInfo.InvariantCulture);
                    DateTime horaFimAlmoco = DateTime.ParseExact(horaFimAlmocoStr, formatoHora, CultureInfo.InvariantCulture);

                    // Defina a margem de 5 minutos antes e 15 minutos depois para a entrada e saída
                    TimeSpan margemAntesEntrada = new TimeSpan();
                    TimeSpan margemDepoisEntrada = TimeSpan.FromMinutes(15);



                    // Defina a margem de 5 minutos antes e 15 minutos depois para a saída
                    TimeSpan margemAntesSaida = new TimeSpan();
                    TimeSpan margemDepoisSaida = TimeSpan.FromMinutes(15);



                    TimeSpan margemAntesInicioAlmoco = new TimeSpan();
                    TimeSpan margemDepoisInicioAlmoco = TimeSpan.FromMinutes(15);


                    TimeSpan margemAntesFimAlmoco = new TimeSpan();
                    TimeSpan margemDepoisFimAlmoco = TimeSpan.FromMinutes(15);



                    bool sextaFeira = horaAtual.DayOfWeek == DayOfWeek.Friday;
                    if (sextaFeira)
                    {
                        margemAntesEntrada = TimeSpan.FromMinutes(60);

                        // Defina a margem de 5 minutos antes e 15 minutos depois para a saída
                        margemAntesSaida = TimeSpan.FromMinutes(60);

                        margemAntesInicioAlmoco = TimeSpan.FromMinutes(60);

                        margemAntesFimAlmoco = TimeSpan.FromMinutes(60);

                    }
                    else
                    {
                        margemAntesEntrada = TimeSpan.FromMinutes(15);

                        // Defina a margem de 5 minutos antes e 15 minutos depois para a saída
                        margemAntesSaida = TimeSpan.FromMinutes(15);

                        margemAntesInicioAlmoco = TimeSpan.FromMinutes(15);

                        margemAntesFimAlmoco = TimeSpan.FromMinutes(15);
                    }

                    // Verifique se a hora atual está dentro da margem de 5 minutos antes da hora de entrada e 15 minutos depois da hora de entrada
                    bool dentroMargemEntrada = ((horaAtual >= horaEntrada.Add(-margemAntesEntrada)) && (horaAtual <= horaEntrada.Add(margemDepoisEntrada)));

                    // Verifique se a hora atual está dentro da margem de 5 minutos antes da hora de saída e 15 minutos depois da hora de saída
                    bool dentroMargemSaida = ((horaAtual >= horaSaida.Add(-margemAntesSaida)) && (horaAtual <= horaSaida.Add(margemDepoisSaida)));

                    bool dentroMargemInicioAlmoco = ((horaAtual >= horaInicioAlmoco.Add(-margemAntesInicioAlmoco)) && (horaAtual <= horaInicioAlmoco.Add(margemDepoisInicioAlmoco)));

                    bool dentroMargemFimAlmoco = ((horaAtual >= horaFimAlmoco.Add(-margemAntesFimAlmoco)) && (horaAtual <= horaFimAlmoco.Add(margemDepoisFimAlmoco)));

                    // Verifique se a hora atual está dentro da margem permitida para a entrada ou saída
                    if (dentroMargemEntrada || dentroMargemSaida || dentroMargemInicioAlmoco || dentroMargemFimAlmoco)
                    {
                        string sql = @"
                                SELECT *
                                    FROM (
                                        SELECT ou.*, 
                                               o.Latitude AS ObraLatitude, 
                                               o.Longitude AS ObraLongitude,
                                               (6371 * 1000 *
                                                ACOS(
                                                    COS(RADIANS({0})) *
                                                    COS(RADIANS(o.Latitude)) *
                                                    COS(RADIANS({1}) - RADIANS(o.Longitude)) +
                                                    SIN(RADIANS({0})) *
                                                    SIN(RADIANS(o.Latitude))
                                                )) AS distance
                                        FROM ObraUsuario ou
                                        INNER JOIN Obra o ON ou.IDObra = o.IDObra
                                        WHERE ou.IDUsuario = {2}
                                    ) AS dr
                                    WHERE dr.distance < {3};
                                    ";


                        // Preencher os parâmetros da consulta SQL
                        string formattedSql = string.Format(sql, obj.Latitude, obj.Longitude, usuario.Idusuario, 100); // Valor padrão de 50 para o raio

                        var clientesProximos = context.ObraUsuario.FromSqlRaw(formattedSql).ToList();

                        if (clientesProximos.Count > 0)
                        {
                            if (ponto == null)
                            {
                                var newPonto = new Ponto();
                                if (obj.TipoMarcacao == EnumTipoMarcacao.InicioDia.ID)
                                {
                                    newPonto.DataPonto = DateTime.Now.Date;
                                    newPonto.Idusuario = usuario.Idusuario;
                                    newPonto.InicioDia = DateTime.Now;
                                    newPonto.InicioDiaLat = new Util().GetCoordenada(obj.Latitude);
                                    newPonto.InicioDiaLong = new Util().GetCoordenada(obj.Longitude);
                                }
                                else if (obj.TipoMarcacao == EnumTipoMarcacao.InicioAlmoco.ID)
                                {
                                    return Erro("Para iniciar o almoço é necessário iniciar o dia!");
                                }
                                else if (obj.TipoMarcacao == EnumTipoMarcacao.FimAlmoco.ID)
                                {
                                    return Erro("Para finalizar o almoço é necessario iniciar o mesmo!");
                                }
                                else if (obj.TipoMarcacao == EnumTipoMarcacao.FimDia.ID)
                                {
                                    return Erro("Para finalizar o dia é necessario iniciar o mesmo!");
                                }
                                context.Ponto.Add(newPonto);
                                context.SaveChanges();
                            }
                            else
                            {
                                if (ponto.InicioDia == null && obj.TipoMarcacao == EnumTipoMarcacao.InicioDia.ID)
                                {
                                    ponto.InicioDia = DateTime.Now;
                                    ponto.InicioDiaLat = new Util().GetCoordenada(obj.Latitude);
                                    ponto.InicioDiaLong = new Util().GetCoordenada(obj.Longitude);
                                }
                                else if (ponto.InicioAlmoco == null && obj.TipoMarcacao == EnumTipoMarcacao.InicioAlmoco.ID)
                                {
                                    ponto.InicioAlmoco = DateTime.Now;
                                    ponto.InicioAlmocoLat = new Util().GetCoordenada(obj.Latitude);
                                    ponto.InicioAlmocoLong = new Util().GetCoordenada(obj.Longitude);
                                }
                                else if (ponto.FimAlmoco == null && obj.TipoMarcacao == EnumTipoMarcacao.FimAlmoco.ID)
                                {
                                    if (ponto.InicioAlmoco == null)
                                    {
                                        return Erro("Para finalizar o almoço é necessario iniciar o mesmo!");
                                    }

                                    ponto.FimAlmoco = DateTime.Now;
                                    ponto.FimAlmocoLat = new Util().GetCoordenada(obj.Latitude);
                                    ponto.FimAlmocoLong = new Util().GetCoordenada(obj.Longitude);
                                }
                                else if (ponto.FimDia == null && obj.TipoMarcacao == EnumTipoMarcacao.FimDia.ID)
                                {
                                    if (ponto.InicioDia == null && ponto.FimAlmoco == null)
                                    {
                                        return Erro("Para finalizar o dia é necessario iniciar o mesmo!");
                                    }

                                    ponto.FimDia = DateTime.Now;
                                    ponto.FimDiaLat = new Util().GetCoordenada(obj.Latitude);
                                    ponto.FimDiaLong = new Util().GetCoordenada(obj.Longitude);
                                }
                                context.Entry(ponto);
                                context.SaveChanges();
                            }
                        }
                        else
                        {

                            string sqlEmpresa = @"
                                SELECT *
                                    FROM (
                                        SELECT ou.*, 
                                               ou.Latitude AS EmpresaLatitude, 
                                               ou.Longitude AS EmpresaLongitude,
                                               (6371 * 1000 *
                                                ACOS(
                                                    COS(RADIANS({0})) *
                                                    COS(RADIANS(ou.Latitude)) *
                                                    COS(RADIANS({1}) - RADIANS(ou.Longitude)) +
                                                    SIN(RADIANS({0})) *
                                                    SIN(RADIANS(ou.Latitude))
                                                )) AS distance
                                        FROM Empresa ou
                                        WHERE ou.IDEmpresa = {2}
                                    ) AS dr
                                    WHERE dr.distance < {3}
                                    ";


                            // Preencher os parâmetros da consulta SQL
                            string formattedSqlEmpresa = string.Format(sqlEmpresa, obj.Latitude, obj.Longitude, usuario.Idempresa, 100); // Valor padrão de 50 para o raio

                            var Empresa = context.Empresa.FromSqlRaw(formattedSqlEmpresa).FirstOrDefault();

                            if (Empresa != null)
                            {
                                if (ponto == null)
                                {
                                    var newPonto = new Ponto();
                                    if (obj.TipoMarcacao == EnumTipoMarcacao.InicioDia.ID)
                                    {
                                        newPonto.DataPonto = DateTime.Now.Date;
                                        newPonto.Idusuario = usuario.Idusuario;
                                        newPonto.InicioDia = DateTime.Now;
                                        newPonto.InicioDiaLat = new Util().GetCoordenada(obj.Latitude);
                                        newPonto.InicioDiaLong = new Util().GetCoordenada(obj.Longitude);
                                    }
                                    else if (obj.TipoMarcacao == EnumTipoMarcacao.InicioAlmoco.ID)
                                    {
                                        newPonto.DataPonto = DateTime.Now.Date;
                                        newPonto.Idusuario = usuario.Idusuario;
                                        newPonto.InicioAlmoco = DateTime.Now;
                                        newPonto.InicioAlmocoLat = new Util().GetCoordenada(obj.Latitude);
                                        newPonto.InicioAlmocoLong = new Util().GetCoordenada(obj.Longitude);
                                    }
                                    else if (obj.TipoMarcacao == EnumTipoMarcacao.FimAlmoco.ID)
                                    {
                                        return Erro("Para finalizar o almoço é necessario iniciar o mesmo!");
                                    }
                                    else if (obj.TipoMarcacao == EnumTipoMarcacao.FimDia.ID)
                                    {
                                        return Erro("Para finalizar o dia é necessario iniciar o mesmo!");
                                    }
                                    context.Ponto.Add(newPonto);
                                    context.SaveChanges();
                                }
                                else
                                {
                                    if (ponto.InicioDia == null && obj.TipoMarcacao == EnumTipoMarcacao.InicioDia.ID)
                                    {
                                        ponto.InicioDia = DateTime.Now;
                                        ponto.InicioDiaLat = new Util().GetCoordenada(obj.Latitude);
                                        ponto.InicioDiaLong = new Util().GetCoordenada(obj.Longitude);
                                    }
                                    else if (ponto.InicioAlmoco == null && obj.TipoMarcacao == EnumTipoMarcacao.InicioAlmoco.ID)
                                    {
                                        ponto.InicioAlmoco = DateTime.Now;
                                        ponto.InicioAlmocoLat = new Util().GetCoordenada(obj.Latitude);
                                        ponto.InicioAlmocoLong = new Util().GetCoordenada(obj.Longitude);
                                    }
                                    else if (ponto.FimAlmoco == null && obj.TipoMarcacao == EnumTipoMarcacao.FimAlmoco.ID)
                                    {
                                        if (ponto.InicioAlmoco == null)
                                        {
                                            return Erro("Para finalizar o almoço é necessario iniciar o mesmo!");
                                        }

                                        ponto.FimAlmoco = DateTime.Now;
                                        ponto.FimAlmocoLat = new Util().GetCoordenada(obj.Latitude);
                                        ponto.FimAlmocoLong = new Util().GetCoordenada(obj.Longitude);
                                    }
                                    else if (ponto.FimDia == null && obj.TipoMarcacao == EnumTipoMarcacao.FimDia.ID)
                                    {
                                        if (ponto.InicioDia == null && ponto.FimAlmoco == null)
                                        {
                                            return Erro("Para finalizar o dia é necessario iniciar o mesmo!");
                                        }

                                        ponto.FimDia = DateTime.Now;
                                        ponto.FimDiaLat = new Util().GetCoordenada(obj.Latitude);
                                        ponto.FimDiaLong = new Util().GetCoordenada(obj.Longitude);
                                    }
                                    context.Entry(ponto);
                                    context.SaveChanges();
                                }
                            }
                            else
                            {
                                return Erro("Não foi possivel realizar a marcação do ponto, você não esta proximo a nenhuma obra ou empresa!!");
                            }
                        }
                    }
                    else
                    {
                        return Erro("Não foi possível realizar a marcação do ponto, fora do período liberado!!");
                    }
                }

                return ConsultarPontoDia();
            }
            catch (Exception ex)
            {
                return Erro(ex);
            }
        }

        public object ConsultarHistoricoPonto()
        {
            try
            {
                var context = new ultimate_mindContext();
                var usuario = GetUsuarioLogado();

                var ponto = context.Ponto.Where(r => r.Idusuario == usuario.IDUsuario).Take(30).ToList();

                var retorno = new List<PontoMarcadoDTO>();
                if (ponto.Count > 0)
                {
                    foreach (var item in ponto)
                    {
                        var newRetorno = new PontoMarcadoDTO();
                        newRetorno.DataAtual = item.DataPonto.ToString("dd/MM/yyyy");
                        newRetorno.InicioDia = item.InicioDia == null ? null : item.InicioDia.Value.ToString("HH:mm");
                        newRetorno.InicioAlmoco = item.InicioAlmoco == null ? null : item.InicioAlmoco.Value.ToString("HH:mm");
                        newRetorno.FimAlmoco = item.FimAlmoco == null ? null : item.FimAlmoco.Value.ToString("HH:mm");
                        newRetorno.FimDia = item.FimDia == null ? null : item.FimDia.Value.ToString("HH:mm");

                        // Calcular a jornada de trabalho do dia
                        TimeSpan jornada = TimeSpan.Zero;
                        DateTime limiteJornada = item.DataPonto.Date.AddDays(1); // Limite é o próximo dia 00:00

                        if (item.InicioDia != null)
                        {
                            if (item.FimDia != null)
                            {
                                jornada = item.FimDia.Value - item.InicioDia.Value;
                            }
                            else
                            {
                                if (item.InicioAlmoco != null)
                                {
                                    if (item.FimAlmoco != null)
                                    {
                                        jornada = item.FimAlmoco.Value - item.InicioDia.Value;
                                    }
                                    else
                                    {
                                        jornada = item.InicioAlmoco.Value - item.InicioDia.Value;
                                    }
                                }

                                jornada = limiteJornada - item.InicioDia.Value;
                            }
                        }

                        newRetorno.Jornada = jornada.ToString(@"hh\:mm");

                        retorno.Add(newRetorno);
                    }
                }

                return retorno;
            }
            catch (Exception ex)
            {
                return Erro(ex);
            }
        }

        public object ConsultarLiberacaoPonto()
        {
            try
            {
                var context = new ultimate_mindContext();
                var usuario = GetUsuarioLogado();

                var liberacaoPonto = context.HistoricoLiberacaoPonto
                    .Include(r => r.IdusuarioLiberadoNavigation)
                    .Include(r => r.IdusuarioGestorNavigation)
                    .Where(r => r.IdusuarioLiberadoNavigation.Idempresa == usuario.IDEmpresaLogon && r.IdusuarioLiberadoNavigation.IsLiberacaoPonto == true)
                    .Select(r => new
                    {
                        r.IdhistoricoLiberacaoPonto,
                        NomeGestor = r.IdusuarioGestorNavigation.Nome,
                        NomeColaborador = r.IdusuarioLiberadoNavigation.Nome,
                        Data = r.DataHoraLiberacao.ToString("dd/MM/yyyy"),
                        Hora = r.DataHoraLiberacao.ToString("HH:mm"),
                        Observacao = r.ObservacaoLiberacao
                    }).ToList();

                return liberacaoPonto;
            }
            catch (Exception ex)
            {
                return Erro(ex);
            }
        }

        public object SalvarLiberacaoPonto(int usuario, string observacao)
        {
            try
            {
                var context = new ultimate_mindContext();
                var usuarioLogado = GetUsuarioLogado();

                var usuarioLiberacao = context.Usuario.Where(r => r.Idusuario == usuario).FirstOrDefault();

                if (usuarioLiberacao.IsLiberacaoPonto != true)
                {
                    context.HistoricoLiberacaoPonto.Add(new HistoricoLiberacaoPonto
                    {
                        IdusuarioGestor = usuarioLogado.IDUsuario,
                        IdusuarioLiberado = usuarioLiberacao.Idusuario,
                        DataHoraLiberacao = DateTime.Now,
                        ObservacaoLiberacao = observacao
                    });

                    usuarioLiberacao.IsLiberacaoPonto = true;
                    context.Entry(usuarioLiberacao);
                    context.SaveChanges();

                    return Ok();
                }
                else
                    return Erro("Usuário já esta liberado para realizar a marcação de ponto!");
            }
            catch (Exception ex)
            {
                return Erro(ex);
            }
        }

        public object ExcluirLiberacaoPonto(int id)
        {
            try
            {
                var context = new ultimate_mindContext();

                var liberacaoPonto = context.HistoricoLiberacaoPonto.Where(r => r.IdhistoricoLiberacaoPonto == id).FirstOrDefault();

                if (liberacaoPonto != null)
                {
                    var usuario = context.Usuario.Where(r => r.Idusuario == liberacaoPonto.IdusuarioLiberado).FirstOrDefault();
                    usuario.IsLiberacaoPonto = false;
                    context.Remove(liberacaoPonto);
                    context.Entry(usuario);
                    context.SaveChanges();
                    return Ok();
                }
                else
                    return Erro("Liberação de Ponto, não encontrada");
            }
            catch (Exception ex)
            {
                return Erro(ex);
            }
        }
    }
}
