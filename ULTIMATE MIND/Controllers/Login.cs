using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using ULTIMATE_MIND.Arquitetura.DTO;

namespace ULTIMATE_MIND.Controllers
{
    public class Login : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }

    //[HttpPost]
    //public object Logar(UsuarioLogin usuario)
    //{
    //    try
    //    {
    //        var context = new PortalContext();
    //        long bCpf = new Util().ToCpf(usuario.Cpf);

    //        var userBanco = context.Usuario
    //            .Include(r => r.IdempresaLogonNavigation)
    //            .Where(r => r.Cpf == bCpf
    //                && r.Status == EnumStatusUsuario.Ativo.ID)
    //            .FirstOrDefault();

    //        if (userBanco != null)
    //        {
    //            if (userBanco.Senha != new Util().GetHashMD5(userBanco.Idusuario + usuario.Senha))
    //                return this.Erro("Usuário ou senha inválidos");
    //        }
    //        else
    //        {
    //            return this.Erro("Usuário ou senha não encontrado.");
    //        }

    //        var empresaLogon = context.Empresa.Where(r => r.Idempresa == userBanco.IdempresaLogon).FirstOrDefault();

    //        if (empresaLogon == null || empresaLogon.Idempresa <= 0)
    //            return this.Erro("Usuário não possui empresa padrão configurada!");

    //        usuario.IDUsuario = userBanco.Idusuario;
    //        usuario.IDEmpresaLogon = userBanco.IdempresaLogon;
    //        usuario.IDGrupoUsuario = userBanco.IdgrupoUsuario == null ? 0 : userBanco.IdgrupoUsuario.Value;
    //        usuario.Nome = userBanco.Nome;
    //        usuario.ApelidoEmpresa = userBanco.IdempresaLogonNavigation.Apelido;
    //        usuario.NomeFantasiaEmpresa = userBanco.IdempresaLogonNavigation.NomeFantasia;
    //        usuario.GUID = userBanco.Guid;
    //        new Util().AddClaim(this, Constantes.UsuarioLogado, JsonConvert.SerializeObject(usuario));

    //        int listRetrno = 1;
    //        listRetrno += context.UsuarioEmpresa
    //            .Include(r => r.IdempresaNavigation)
    //            .Where(r => r.Idusuario == userBanco.Idusuario)
    //            .Select(r => r.IdempresaNavigation)
    //            .Count();

    //        List<string> listaPermissoes = new List<string>();

    //        if (listRetrno == 1)
    //        {
    //            Response.Cookies.Append(Constantes.EmpresaSelecionada, userBanco.IdempresaLogon.ToString());
    //            listaPermissoes.AddRange(context.GrupoUsuarioPermissoes
    //                .Where(r => r.IdgrupoUsuario == userBanco.IdgrupoUsuario && r.Idempresa == userBanco.IdempresaLogon)
    //            .Select(r => r.Idtela));

    //            new Util().AddClaim(this, Constantes.UsuarioPermissoes, JsonConvert.SerializeObject(listaPermissoes));
    //        }

    //        return listRetrno;
    //    }
    //    catch (System.Exception e)
    //    {
    //        return this.Erro(e.Message);
    //    }
    //}
}
