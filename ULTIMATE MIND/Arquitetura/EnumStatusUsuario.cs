﻿using System.Collections.Generic;

namespace ULTIMATE_MIND.Arquitetura
{
    public class EnumStatusUsuario
    {
        private static List<EnumStatusUsuario> listaInterna = new List<EnumStatusUsuario>();

        public static EnumStatusUsuario Inativo = new EnumStatusUsuario(0, "Inativo");
        public static EnumStatusUsuario Ativo = new EnumStatusUsuario(1, "Ativo");
        public static EnumStatusUsuario Ferias = new EnumStatusUsuario(2, "Férias");

        public EnumStatusUsuario()
        {
        }

        private EnumStatusUsuario(int ID, string Nome)
            : this()
        {
            this.ID = ID;
            this.Nome = Nome;
            listaInterna.Add(this);
        }

        private EnumStatusUsuario(int ID, string Nome, bool pExcluir)
            : this()
        {
            this.ID = ID;
            this.Nome = Nome;
        }

        public int ID { get; set; }
        public string Nome { get; set; }

        public static List<EnumStatusUsuario> ObtenhaLista()
        {
            return listaInterna;
        }

        public static EnumStatusUsuario Obtenha(int codigo)
        {
            foreach (EnumStatusUsuario item in listaInterna)
            {
                if (item.ID == codigo) return item;
            }
            return null;
        }

        public override bool Equals(object obj)
        {
            return obj != null && typeof(EnumStatusUsuario) == obj.GetType() && ((EnumStatusUsuario)obj).ID == ID;
        }

        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }

        public override string ToString()
        {
            return this.Nome.ToString();
        }
    }
}
