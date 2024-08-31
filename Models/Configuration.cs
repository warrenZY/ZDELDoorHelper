using System.Xml.Serialization;
namespace ZDELDoorHelper.Models;

public class Configuration
{
    public string MobileNumber { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}


[XmlRoot("Configurations")]
public class ConfigurationXml
{
    [XmlElement("Configuration")]
    public Configuration[]? Configurations { get; set; }
}