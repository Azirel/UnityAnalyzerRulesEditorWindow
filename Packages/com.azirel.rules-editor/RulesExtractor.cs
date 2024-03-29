﻿using System;
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
		private const string ExtractorLocalPath = "./Packages/com.azirel.rules-editor/.RulesExtractorCLI/RulesExtracorCLI";
		private const string CachedJsonKey = "AnalyzersJsonCache";
		private static readonly string extractorPath;
		private static string ExtractorPath => String.IsNullOrEmpty(extractorPath) ? Path.GetFullPath(ExtractorLocalPath) : extractorPath;
		private static string dotnetPath;

		public static void CacheRules(ILookup<string, AnalyzerRule> rules)
			=> EditorPrefs.SetString(CachedJsonKey, JsonConvert.SerializeObject(rules.SelectMany(group => group.ToList())));

		public static ILookup<string, AnalyzerRule> GetCachedRules()
		{
			var rules = JsonConvert.DeserializeObject<List<AnalyzerRule>>(EditorPrefs.GetString(CachedJsonKey));
			return EditorPrefs.HasKey(CachedJsonKey)
				? JsonConvert.DeserializeObject<List<AnalyzerRule>>(EditorPrefs.GetString(CachedJsonKey))
			.ToLookup(keySelector: rule => rule.AnalyzerId, elementSelector: rule => rule)
				: Utilities.EmptyLookup<string, AnalyzerRule>();
		}

		public static ILookup<string, AnalyzerRule> ExtractRules()
			=> AssetDatabase.FindAssets(AnalyzersFilter)
				.Select(guid => (guid, ExtractDescriptorsFromSingleAnalyzersLibrary(guid)))
				.Where(rules => rules.Item2?.Any() == true)
				.SelectMany(group => group.Item2,
				(group, descriptor) => (name: AssetNameFromGUID(group.guid), rule: new AnalyzerRule(descriptor, AssetNameFromGUID(group.guid))))
				.ToLookup(analyzerName => analyzerName.name, item => item.rule);

		private static IEnumerable<DiagnosticDescriptorEssentials> ExtractDescriptorsFromSingleAnalyzersLibrary(string assetGUID)
		{
			try
			{
				var assetPath = AssetDatabase.GUIDToAssetPath(assetGUID);
				var assetFullPath = Path.GetFullPath(assetPath);
				var analyzerJson = StartProcessAndGetOutput(ExtractorPath, assetFullPath);
				var result = JsonConvert.DeserializeObject<IEnumerable<DiagnosticDescriptorEssentials>>(analyzerJson);
				result = result?.Any() == true ? result : Enumerable.Empty<DiagnosticDescriptorEssentials>();
				return result;
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

#if UNITY_EDITOR_WIN
		private static string StartProcessAndGetOutput(string exePath, string arguments)
		{
			using var extractorProcess = new Process();
			extractorProcess.StartInfo.UseShellExecute = false;
			extractorProcess.StartInfo.RedirectStandardOutput = true;
			extractorProcess.StartInfo.FileName = exePath + ".exe";
			extractorProcess.StartInfo.Arguments = arguments;
			extractorProcess.StartInfo.CreateNoWindow = true;
			_ = extractorProcess.Start();
			using var output = extractorProcess.StandardOutput;
			var outputText = output.ReadToEnd();
			extractorProcess.WaitForExit();
			return outputText;
		}
#else
		private static string GetDotnetPath()
		{
			using var extractorProcess = new Process();
			extractorProcess.StartInfo.UseShellExecute = false;
			extractorProcess.StartInfo.RedirectStandardOutput = true;
			extractorProcess.StartInfo.FileName = "/bin/bash";
			extractorProcess.StartInfo.Arguments = "-lc \"which dotnet\"";
			extractorProcess.StartInfo.CreateNoWindow = true;
			_ = extractorProcess.Start();
			using var output = extractorProcess.StandardOutput;
			var outputText = output.ReadToEnd();
			extractorProcess.WaitForExit();
			return outputText.TrimEnd('\n', 'r');
		}

		private static string StartProcessAndGetOutput(string exePath, string arguments)
		{
			if (String.IsNullOrEmpty(dotnetPath))
				dotnetPath = GetDotnetPath();

			using var extractorProcess = new Process();
			extractorProcess.StartInfo.UseShellExecute = false;
			extractorProcess.StartInfo.RedirectStandardOutput = true;
			extractorProcess.StartInfo.FileName = dotnetPath;
			extractorProcess.StartInfo.ArgumentList.Add(exePath + ".dll");
			extractorProcess.StartInfo.ArgumentList.Add(arguments);
			extractorProcess.StartInfo.CreateNoWindow = true;
			_ = extractorProcess.Start();
			using var output = extractorProcess.StandardOutput;
			var outputText = output.ReadToEnd();
			extractorProcess.WaitForExit();
			return outputText;
		}
#endif
	}
}
