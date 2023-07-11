using System.Collections.Generic;

namespace ULTIMATE_MIND.Arquitetura.Enum
{
    public class EnumStatusObra
    {
        private static List<EnumStatusObra> listaInterna = new List<EnumStatusObra>();

        public static EnumStatusObra Ativo = new EnumStatusObra(1, "Ativo");
        public static EnumStatusObra Inativo = new EnumStatusObra(2, "Inativo");
        public static EnumStatusObra Finalizada = new EnumStatusObra(3, "Finalizada");

        public EnumStatusObra()
        {
        }

        private EnumStatusObra(int ID, string Nome)
            : this()
        {
            this.ID = ID;
            this.Nome = Nome;
            listaInterna.Add(this);
        }

        private EnumStatusObra(int ID, string Nome, bool pExcluir)
            : this()
        {
            this.ID = ID;
            this.Nome = Nome;
        }

        public int ID { get; set; }
        public string Nome { get; set; }

        public static List<EnumStatusObra> ObtenhaLista()
        {
            return listaInterna;
        }

        public static string Obtenha(int codigo)
        {
            foreach (EnumStatusObra item in listaInterna)
            {
                if (item.ID == codigo) return item.Nome;
            }
            return null;
        }

        public override bool Equals(object obj)
        {
            return obj != null && typeof(EnumStatusObra) == obj.GetType() && ((EnumStatusObra)obj).ID == ID;
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
