using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using System.Xml.Serialization;

namespace Azirel
{
	public static class RulesetIO
	{
		public static IEnumerable<(string id, DiagnosticSeverity severity)> LoadRulesetFile(string path)
		{
			var serializer = new XmlSerializer(typeof(XmlRuleSetNodeSchema));
			using var reader = File.OpenRead(path);
			return ((XmlRuleSetNodeSchema)serializer.Deserialize(reader)).RuleLists
				.SelectMany(rules => rules.Rules
				.Select(rule => (rule.Id, rule.Severity)));
		}

		public static void SaveTo(string ruleSetPath, ILookup<string, AnalyzerRule> analyzers)
		{
			var ruleSetName = Path.GetFileName(ruleSetPath);
			var serializer = new XmlSerializer(typeof(XmlRuleSetNodeSchema));
			var ruleSet = new XmlRuleSetNodeSchema()
			{
				Name = ruleSetName,
				Description = String.Empty,
				ToolsVersion = 1.0d,
				RuleLists = analyzers.Select(group =>
					new XmlRulesListNodeSchema()
					{
						AnalyzerId = group.Key,
						RuleNamespace = group.Key,
						Rules = group.Select(rule => new XmlRuleNodeSchema()
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
}
