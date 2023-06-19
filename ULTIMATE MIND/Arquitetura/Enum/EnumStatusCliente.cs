using System.Collections.Generic;

namespace ULTIMATE_MIND.Arquitetura.Enum
{
    public class EnumStatusCliente
    {
        private static List<EnumStatusCliente> listaInterna = new List<EnumStatusCliente>();
        
        public static EnumStatusCliente Ativo = new EnumStatusCliente(1, "Ativo");
        public static EnumStatusCliente Inativo = new EnumStatusCliente(2, "Inativo");

        public EnumStatusCliente()
        {
        }

        private EnumStatusCliente(int ID, string Nome)
            : this()
        {
            this.ID = ID;
            this.Nome = Nome;
            listaInterna.Add(this);
        }

        private EnumStatusCliente(int ID, string Nome, bool pExcluir)
            : this()
        {
            this.ID = ID;
            this.Nome = Nome;
        }

        public int ID { get; set; }
        public string Nome { get; set; }

        public static List<EnumStatusCliente> ObtenhaLista()
        {
            return listaInterna;
        }

        public static string Obtenha(int codigo)
        {
            foreach (EnumStatusCliente item in listaInterna)
            {
                if (item.ID == codigo) return item.Nome;
            }
            return null;
        }

        public override bool Equals(object obj)
        {
            return obj != null && typeof(EnumStatusCliente) == obj.GetType() && ((EnumStatusCliente)obj).ID == ID;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override string ToString()
        {
            return Nome.ToString();
        }
    }
}
