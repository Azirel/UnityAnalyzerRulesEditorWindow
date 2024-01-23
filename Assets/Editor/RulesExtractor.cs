using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;

namespace Azirel
{
	public class RulesExtractor
	{
		private const string AnalyzersFilter = "l:RoslynAnalyzer";
		private const string AnalyzersDependenciesFilter = "l:RoslynAnalyzerDependency";
		private const string ExtractorLocalPath = @"Assets\.RulesExtractorCLI\RulesExtracorCLI.exe";
		private const string CachedJsonKey = "AnalyzersJsonCache";
		private static readonly string extractorPath;
		private static string ExtractorPath => String.IsNullOrEmpty(extractorPath) ? Path.GetFullPath(ExtractorLocalPath) : extractorPath;

		public static void CacheRules(ILookup<string, AnalyzerRule> rules)
			=> EditorPrefs.SetString(CachedJsonKey, JsonConvert.SerializeObject(rules.SelectMany(group => group.ToList())));

		public static ILookup<string, AnalyzerRule> GetCachedRules()
		{
			var rules = JsonConvert.DeserializeObject<List<AnalyzerRule>>(EditorPrefs.GetString(CachedJsonKey));
			return 	EditorPrefs.HasKey(CachedJsonKey)
				? JsonConvert.DeserializeObject<List<AnalyzerRule>>(EditorPrefs.GetString(CachedJsonKey))
			.ToLookup(keySelector: rule => rule.AnalyzerId, elementSelector: rule => rule)
				: Utilities.EmptyLookup<string, AnalyzerRule>();
		}

		public static ILookup<string, AnalyzerRule> ExtractRules()
			=> AssetDatabase.FindAssets(AnalyzersFilter)
				.Except(AssetDatabase.FindAssets(AnalyzersDependenciesFilter))
				.SelectMany(ExtractDescriptorsFromSingleAnalyzersLibrary,
				(guid, descriptor) => (name: AssetNameFromGUID(guid), rule: new AnalyzerRule(descriptor, AssetNameFromGUID(guid))))
				.ToLookup(analyerName => analyerName.name, item => item.rule);

		private static IEnumerable<DiagnosticDescriptorEssentials> ExtractDescriptorsFromSingleAnalyzersLibrary(string assetGUID)
		{
			try
			{
				var assetPath = AssetDatabase.GUIDToAssetPath(assetGUID);
				var assetFullPath = Path.GetFullPath(assetPath);
				var analyzerJson = StartProcessAndGetOutput(ExtractorPath, assetFullPath);
				return JsonConvert.DeserializeObject<IEnumerable<DiagnosticDescriptorEssentials>>(analyzerJson);
			}
			catch (Exception exception)
			{
				UnityEngine.Debug.LogError(exception);
				return Enumerable.Empty<DiagnosticDescriptorEssentials>();
			}
		}

		private static string AssetNameFromGUID(string guid)
		{
			var assetPath = AssetDatabase.GUIDToAssetPath(guid);
			return Path.GetFileNameWithoutExtension(assetPath);
		}

		private static string StartProcessAndGetOutput(string exePath, string arguments)
		{
			using var extractorProcess = new Process();
			extractorProcess.StartInfo.UseShellExecute = false;
			extractorProcess.StartInfo.RedirectStandardOutput = true;
			extractorProcess.StartInfo.FileName = exePath;
			extractorProcess.StartInfo.Arguments = arguments;
			extractorProcess.StartInfo.CreateNoWindow = true;
			_ = extractorProcess.Start();
			using var output = extractorProcess.StandardOutput;
			var outputText = output.ReadToEnd();
			extractorProcess.WaitForExit();
			return outputText;
		}
	}
}
