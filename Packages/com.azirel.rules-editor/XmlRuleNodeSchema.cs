using System.Xml;
using System.Xml.Serialization;

namespace Azirel
{
	[XmlRoot(ElementName = "Rule")]
	public class XmlRuleNodeSchema
	{
		[XmlAttribute(AttributeName = "Id")]
		public string Id { get; set; }

		[XmlAttribute(AttributeName = "Action")]
		public DiagnosticSeverity Severity { get; set; }
	}
}
