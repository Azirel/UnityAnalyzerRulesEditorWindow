using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Azirel
{
	[XmlRoot(ElementName = "RuleSet")]
	public class XmlRuleSetNodeSchema
	{
		[XmlElement(ElementName = "Rules")]
		public List<XmlRulesListNodeSchema> RuleLists { get; set; }

		[XmlAttribute(AttributeName = "Name")]
		public string Name { get; set; }

		[XmlAttribute(AttributeName = "Description")]
		public string Description { get; set; }

		[XmlAttribute(AttributeName = "ToolsVersion")]
		public double ToolsVersion { get; set; }
	}
}