using System.Collections.Generic;

namespace ULTIMATE_MIND.Arquitetura.Enum
{
    public class EnumStatusEmpresa
    {
        private static List<EnumStatusEmpresa> listaInterna = new List<EnumStatusEmpresa>();
        
        public static EnumStatusEmpresa Ativo = new EnumStatusEmpresa(1, "Ativo");
        public static EnumStatusEmpresa Inativo = new EnumStatusEmpresa(2, "Inativo");

        public EnumStatusEmpresa()
        {
        }

        private EnumStatusEmpresa(int ID, string Nome)
            : this()
        {
            this.ID = ID;
            this.Nome = Nome;
            listaInterna.Add(this);
        }

        private EnumStatusEmpresa(int ID, string Nome, bool pExcluir)
            : this()
        {
            this.ID = ID;
            this.Nome = Nome;
        }

        public int ID { get; set; }
        public string Nome { get; set; }

        public static List<EnumStatusEmpresa> ObtenhaLista()
        {
            return listaInterna;
        }

        public static string Obtenha(int codigo)
        {
            foreach (EnumStatusEmpresa item in listaInterna)
            {
                if (item.ID == codigo) return item.Nome;
            }
            return null;
        }

        public override bool Equals(object obj)
        {
            return obj != null && typeof(EnumStatusEmpresa) == obj.GetType() && ((EnumStatusEmpresa)obj).ID == ID;
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
