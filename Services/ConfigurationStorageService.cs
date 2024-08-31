using System.Xml;
using System.Xml.Serialization;
using ZDELDoorHelper.Models;

namespace ZDELDoorHelper.Services;

public class ConfigurationStorageService
{

    public const string ConfigFileName = "ZDELDoorHelperConfiguration.xml";
    public static readonly string ConfigFilePath = Path.Combine(Environment
        .GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ConfigFileName);

    //Read list from XML using stream
    public static List<Configuration> GetListsFromXml()
    {
        using var configFileStream = new FileStream(ConfigFilePath, FileMode.OpenOrCreate);
        XmlSerializer serializer = new XmlSerializer(typeof(ConfigurationXml));
        ConfigurationXml? result = (ConfigurationXml?)serializer.Deserialize(configFileStream);
        var configList = result?.Configurations?.ToList();
        return configList ?? new List<Configuration>();
    }

    //Wirte list to XML using DOM
    public static void WriteListToXml(List<Configuration> list)
    {
        
        XmlDocument xmlDoc = new XmlDocument();
        XmlElement rootElement = xmlDoc.CreateElement("Configurations");
        xmlDoc.AppendChild(rootElement);
        foreach (Configuration config in list)
        {
            XmlElement element = xmlDoc.CreateElement("Configuration");
            XmlElement elementMobileNumber = xmlDoc.CreateElement("MobileNumber");
            XmlElement elementDescription = xmlDoc.CreateElement("Description");
            elementMobileNumber.InnerText= config.MobileNumber;
            elementDescription.InnerText = config.Description;
            element.AppendChild(elementMobileNumber);
            element.AppendChild(elementDescription);
            rootElement.AppendChild(element);
        }
        xmlDoc.Save(ConfigFilePath);
        /*
        using var configFileStream = new FileStream(ConfigFilePath, FileMode.OpenOrCreate);
        XmlSerializer serializer = new XmlSerializer(typeof(ConfigurationXml));
        serializer.Serialize(configFileSteam, list);
        */
    }
}
