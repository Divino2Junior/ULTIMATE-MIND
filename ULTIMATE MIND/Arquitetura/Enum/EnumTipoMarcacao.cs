using System.Collections.Generic;

namespace ULTIMATE_MIND.Arquitetura.Enum
{
    public class EnumTipoMarcacao
    {
        private static List<EnumTipoMarcacao> listaInterna = new List<EnumTipoMarcacao>();

        public static EnumTipoMarcacao InicioDia = new EnumTipoMarcacao(1, "Inicio Dia");
        public static EnumTipoMarcacao InicioAlmoco = new EnumTipoMarcacao(2, "Inicio Almoço");
        public static EnumTipoMarcacao FimAlmoco = new EnumTipoMarcacao(3, "Fim Almoço");
        public static EnumTipoMarcacao FimDia = new EnumTipoMarcacao(4, "Fim Dia");
        public EnumTipoMarcacao()
        {
        }

        private EnumTipoMarcacao(int ID, string Nome)
            : this()
        {
            this.ID = ID;
            this.Nome = Nome;
            listaInterna.Add(this);
        }

        private EnumTipoMarcacao(int ID, string Nome, bool pExcluir)
            : this()
        {
            this.ID = ID;
            this.Nome = Nome;
        }

        public int ID { get; set; }
        public string Nome { get; set; }

        public static List<EnumTipoMarcacao> ObtenhaLista()
        {
            return listaInterna;
        }

        public static string Obtenha(int codigo)
        {
            foreach (EnumTipoMarcacao item in listaInterna)
            {
                if (item.ID == codigo) return item.Nome;
            }
            return null;
        }

        public override bool Equals(object obj)
        {
            return obj != null && typeof(EnumTipoMarcacao) == obj.GetType() && ((EnumTipoMarcacao)obj).ID == ID;
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
