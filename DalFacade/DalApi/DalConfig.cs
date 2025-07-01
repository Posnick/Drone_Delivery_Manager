using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DalApi
{
    class DalConfig
    {
        internal static string DalName;
        internal static Dictionary<string, string> DalPackages;
        internal static string Class;
        internal static string NameSpace;
        static DalConfig()
        {
            XElement dalConfig = XElement.Load(@"xml\dal-config.xml");
            DalName = dalConfig.Element("dal").Value;
            DalPackages = (from pkg in dalConfig.Element("dal-packages").Elements()
                           select pkg
            ).ToDictionary(p => "" + p.Name, p => p.Value);
            Class = dalConfig.Element("dal-packages").Element(DalName).Attribute("class").Value;
            NameSpace = dalConfig.Element("dal-packages").Element(DalName).Attribute("namespace").Value;
        }
    }

    public class DalConfigException : Exception
    {
        public DalConfigException() { }
        public DalConfigException(string message) : base(message) { }
    }
}
