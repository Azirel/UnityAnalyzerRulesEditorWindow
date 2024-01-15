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

	private IEnumerable<DiagnosticDescriptorEssentials> extractedDescriptors = Enumerable.Empty<DiagnosticDescriptorEssentials>();

	public static IEnumerable<DiagnosticDescriptorEssentials> ExtractDescriptors()
	{
		var roslynAnalyzersGUIDS = AssetDatabase.FindAssets(AnalyzersFilter)
			.Except(AssetDatabase.FindAssets(AnalyzersDependenciesFilter));
		var extractorPath = Path.GetFullPath(ExtractorLocalPath);
		return roslynAnalyzersGUIDS.SelectMany(ExtractDescriptorsFromSingleAnalyzersLibrary).ToList();
	}

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
