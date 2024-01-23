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

public class RulesetIO
{
	public static IEnumerable<(string id, DiagnosticSeverity severity)> LoadRulesetFile(string path)
	{
		var serializer = new XmlSerializer(typeof(RuleSet));
		using var reader = File.OpenRead(path);
		return ((RuleSet)serializer.Deserialize(reader)).RuleLists
			.SelectMany(rules => rules.Rules
			.Select(rule => (rule.Id, rule.Severity)));
	}

	public static void SaveTo(string ruleSetPath, ILookup<string, AnalyzerRule> analyzers)
	{
		var ruleSetName = Path.GetFileName(ruleSetPath);
		var serializer = new XmlSerializer(typeof(RuleSet));
		var ruleSet = new RuleSet()
		{
			Name = ruleSetName,
			Description = String.Empty,
			ToolsVersion = 1.0d,
			RuleLists = analyzers.Select(group =>
				new RulesList()
				{
					AnalyzerId = group.Key,
					RuleNamespace = group.Key,
					Rules = group.Select(rule => new Rule()
					{
						Id = rule.Id,
						Severity = rule.Severity,
					}).ToList(),
				}
			).ToList()
		};
		using var rulesetFileStream = File.Create(Path.GetFullPath(ruleSetPath));
		serializer.Serialize(rulesetFileStream, ruleSet);
		rulesetFileStream.Close();
		AssetDatabase.Refresh();
	}
}

[XmlRoot(ElementName = "Rule")]
public class Rule
{
	[XmlAttribute(AttributeName = "Id")]
	public string Id { get; set; }

	[XmlAttribute(AttributeName = "Action")]
	public DiagnosticSeverity Severity { get; set; }
}

[XmlRoot(ElementName = "Rules")]
public class RulesList
{
	[XmlElement(ElementName = "Rule")]
	public List<Rule> Rules { get; set; }

	[XmlAttribute(AttributeName = "AnalyzerId")]
	public string AnalyzerId { get; set; }

	[XmlAttribute(AttributeName = "RuleNamespace")]
	public string RuleNamespace { get; set; }
}

[XmlRoot(ElementName = "RuleSet")]
public class RuleSet
{
	[XmlElement(ElementName = "Rules")]
	public List<RulesList> RuleLists { get; set; }

	[XmlAttribute(AttributeName = "Name")]
	public string Name { get; set; }

	[XmlAttribute(AttributeName = "Description")]
	public string Description { get; set; }

	[XmlAttribute(AttributeName = "ToolsVersion")]
	public double ToolsVersion { get; set; }
}
