using System.Collections.Generic;
using System.Xml.Serialization;

namespace ULTIMATE_MIND.Models
{
    [XmlRoot("menu")]
    public class MenuModel
    {
        [XmlElement("item")]
        public List<MenuItem> Items { get; set; }
    }

    public class MenuItem
    {
        [XmlAttribute("id")]
        public string TelaId { get; set; }

        [XmlAttribute("tela")]
        public string Tela { get; set; }

        [XmlAttribute("link")]
        public string Link { get; set; }

        [XmlAttribute("type")]
        public int Type { get; set; }

        [XmlArray("submenu")]
        [XmlArrayItem("item")]
        public List<MenuItem> Submenu { get; set; }
    }
}
