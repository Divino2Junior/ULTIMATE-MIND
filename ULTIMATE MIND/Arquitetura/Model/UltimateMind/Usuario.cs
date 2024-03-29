﻿using System;
using System.Collections.Generic;

namespace ULTIMATE_MIND.Arquitetura.Model.UltimateMind
{
    public partial class Usuario
    {
        public Usuario()
        {
            Atendimento = new HashSet<Atendimento>();
            EmpresaUsuario = new HashSet<EmpresaUsuario>();
            HistoricoLiberacaoPontoIdusuarioGestorNavigation = new HashSet<HistoricoLiberacaoPonto>();
            HistoricoLiberacaoPontoIdusuarioLiberadoNavigation = new HashSet<HistoricoLiberacaoPonto>();
            InverseIdusuarioGestorNavigation = new HashSet<Usuario>();
            ObraUsuario = new HashSet<ObraUsuario>();
            Ponto = new HashSet<Ponto>();
            ValidacaoContraCheque = new HashSet<ValidacaoContraCheque>();
        }

        public int Idusuario { get; set; }
        public int Idempresa { get; set; }
        public string Nome { get; set; }
        public string Matricula { get; set; }
        public string Cpf { get; set; }
        public int Idcargo { get; set; }
        public string Email { get; set; }
        public DateTime? DataNascimento { get; set; }
        public int Status { get; set; }
        public string Telefone { get; set; }
        public string Rg { get; set; }
        public int IdgrupoPermissao { get; set; }
        public string Senha { get; set; }
        public DateTime? DataAdmissao { get; set; }
        public DateTime? DataDemissao { get; set; }
        public string NomeFoto { get; set; }
        public string HoraEntrada { get; set; }
        public string HoraSaida { get; set; }
        public bool? IsLiberacaoPonto { get; set; }
        public bool? IsSignatario { get; set; }
        public string ChaveSignatario { get; set; }
        public string HoraInicioAlmoco { get; set; }
        public string HoraFimAlmoco { get; set; }
        public int? IdusuarioGestor { get; set; }

        public virtual Cargo IdcargoNavigation { get; set; }
        public virtual Empresa IdempresaNavigation { get; set; }
        public virtual GrupoPermissao IdgrupoPermissaoNavigation { get; set; }
        public virtual Usuario IdusuarioGestorNavigation { get; set; }
        public virtual ICollection<Atendimento> Atendimento { get; set; }
        public virtual ICollection<EmpresaUsuario> EmpresaUsuario { get; set; }
        public virtual ICollection<HistoricoLiberacaoPonto> HistoricoLiberacaoPontoIdusuarioGestorNavigation { get; set; }
        public virtual ICollection<HistoricoLiberacaoPonto> HistoricoLiberacaoPontoIdusuarioLiberadoNavigation { get; set; }
        public virtual ICollection<Usuario> InverseIdusuarioGestorNavigation { get; set; }
        public virtual ICollection<ObraUsuario> ObraUsuario { get; set; }
        public virtual ICollection<Ponto> Ponto { get; set; }
        public virtual ICollection<ValidacaoContraCheque> ValidacaoContraCheque { get; set; }
    }
}
