using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Azirel
{
	[XmlRoot(ElementName = "Rules")]
	public class XmlRulesListNodeSchema
	{
		[XmlElement(ElementName = "Rule")]
		public List<XmlRuleNodeSchema> Rules { get; set; }

		[XmlAttribute(AttributeName = "AnalyzerId")]
		public string AnalyzerId { get; set; }

		[XmlAttribute(AttributeName = "RuleNamespace")]
		public string RuleNamespace { get; set; }
	}
}
