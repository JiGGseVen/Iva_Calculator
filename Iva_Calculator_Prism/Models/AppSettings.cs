using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Iva_Calculator_Prism.Models
{
    [Serializable]
    [XmlRoot("AppSettings")]
    public class AppSettings
    {
            [XmlArray("CompanySettingsList"), XmlArrayItem(typeof(CompanySettings), ElementName = "CompanySettings")]
            public List<CompanySettings> CompanySettingsList { get; set; }
    }
}
