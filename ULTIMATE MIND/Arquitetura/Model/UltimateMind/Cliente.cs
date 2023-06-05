using System;
using System.Collections.Generic;

namespace ULTIMATE_MIND.Arquitetura.Model.UltimateMind
{
    public partial class Cliente
    {
        public Cliente()
        {
            Atendimento = new HashSet<Atendimento>();
        }

        public int Idcliente { get; set; }
        public string NomeCliente { get; set; }
        public int Idempresa { get; set; }
        public string Cpf { get; set; }
        public string Cnpj { get; set; }
        public int Status { get; set; }
        public string Endereco { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }

        public virtual Empresa IdempresaNavigation { get; set; }
        public virtual ICollection<Atendimento> Atendimento { get; set; }
    }
}
