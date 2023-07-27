using System.Xml.Serialization;

namespace ULTIMATE_MIND.Models
{
    [XmlRoot("menu")]
    public class MenuModel
    {
        [XmlElement("item")]
        public MenuItem[] Items { get; set; }
    }

    public class MenuItem
    {
        [XmlAttribute("tela")]
        public string Tela { get; set; }

        [XmlAttribute("link")]
        public string Link { get; set; }
    }
}
