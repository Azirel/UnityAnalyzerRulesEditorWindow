using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;

public class RulesExtractor
{
	private const string AnalyzersFilter = "l:RoslynAnalyzer";
	private const string AnalyzersDependenciesFilter = "l:RoslynAnalyzerDependency";
	private const string ExtractorLocalPath = @"Assets\.RulesExtractorCLI\RulesExtracorCLI.exe";
	private static string extractorPath;
	private static string ExtractorPath => String.IsNullOrEmpty(extractorPath) ? Path.GetFullPath(ExtractorLocalPath) : extractorPath;
	private const string cachedJsonKey = "AnalyzersJsonCache";

	private IEnumerable<DiagnosticDescriptorEssentials> extractedDescriptors = Enumerable.Empty<DiagnosticDescriptorEssentials>();

	public static void CacheRules(ILookup<string, AnalyzerRule> rules)
		=> EditorPrefs.SetString(cachedJsonKey, JsonConvert.SerializeObject(rules));

	public static ILookup<string, AnalyzerRule> GetCachedRules()
		=> EditorPrefs.HasKey(cachedJsonKey)
			? JsonConvert.DeserializeObject<Lookup<string, AnalyzerRule>>(EditorPrefs.GetString(cachedJsonKey))
			: Utilities.EmptyLookup<string, AnalyzerRule>();

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
		extractorProcess.Start();
		using var output = extractorProcess.StandardOutput;
		var outputText = output.ReadToEnd();
		extractorProcess.WaitForExit();
		return outputText;
	}
}
