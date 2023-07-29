using System.Collections.Generic;
using System.Xml.Serialization;

namespace ULTIMATE_MIND.Models
{
    [XmlRoot("menu")]
    public class MenuModel
    {
        [XmlElement("item")]
        public MenuItem[] Items { get; set; }
    }
    public class SubmenuItem
    {
        public string Tela { get; set; }
        public string Link { get; set; }
    }

    public class MenuItem
    {
        public string Tela { get; set; }
        public string Link { get; set; }
        public List<SubmenuItem> Submenu { get; set; }
    }

}
