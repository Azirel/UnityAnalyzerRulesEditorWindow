using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;
using System.Xml.Serialization;

public class RulesetLoader
{
	public static IEnumerable<AnalyzerRule> LoadRulesetFile()
	{
		var rulesetGUID = AssetDatabase.FindAssets("glob:\"Assets/**.ruleset\"")[0];
		var rulesetPath = Path.GetFullPath(AssetDatabase.GUIDToAssetPath(rulesetGUID));
		//var xDocument = new XmlDocument();
		//xDocument.Load(rulesetPath);
		//File.OpenRead(rulesetPath);
		//foreach (var item in xDocument)
		//{
		//	Debug.Log(item);
		//}

		var serializer = new XmlSerializer(typeof(RuleSet));
		using (var reader = File.OpenRead(rulesetPath))
		{
			var test = (RuleSet)serializer.Deserialize(reader);
		}
		throw new NotImplementedException();
	}
}

[XmlRoot(ElementName = "Rule")]
public class Rule
{

	[XmlAttribute(AttributeName = "Id")]
	public string Id { get; set; }

	[XmlAttribute(AttributeName = "Action")]
	public string Action { get; set; }
}

[XmlRoot(ElementName = "Rules")]
public class Rules
{

	[XmlElement(ElementName = "Rule")]
	public List<Rule> Rule { get; set; }

	[XmlAttribute(AttributeName = "AnalyzerId")]
	public string AnalyzerId { get; set; }

	[XmlAttribute(AttributeName = "RuleNamespace")]
	public string RuleNamespace { get; set; }
}

[XmlRoot(ElementName = "RuleSet")]
public class RuleSet
{

	[XmlElement(ElementName = "Rules")]
	public List<Rules> Rules { get; set; }

	[XmlAttribute(AttributeName = "Name")]
	public string Name { get; set; }

	[XmlAttribute(AttributeName = "Description")]
	public string Description { get; set; }

	[XmlAttribute(AttributeName = "ToolsVersion")]
	public double ToolsVersion { get; set; }
}
