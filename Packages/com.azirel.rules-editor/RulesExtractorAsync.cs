using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace Azirel
{
	public class RulesExtractorAsync
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

		public static async Task<ILookup<string, AnalyzerRule>> ExtractRules()
		{
			var extractors = AssetDatabase.FindAssets(AnalyzersFilter)
				.ToDictionary(guid => guid, elementSelector: guid => new Lazy<Task<IEnumerable<DiagnosticDescriptorEssentials>>>(ExtractDescriptorsFromSingleAnalyzersLibrary(guid)));
			var extractedRules = await Task.WhenAll(extractors.Values.Select(lazyTask => lazyTask.Value));
			return extractors.Where(pair => pair.Value.Value.Result.Any())
				.Select(pair =>  (pair.Key, pair.Value.Value.Result))
				.SelectMany(pair => pair.Result,
				(pair, descriptor) => (name: AssetNameFromGUID(pair.Key), rule: new AnalyzerRule(descriptor, AssetNameFromGUID(pair.Key))))
				.ToLookup(analyzerName => analyzerName.name, item => item.rule);
		}

		private static async Task<IEnumerable<DiagnosticDescriptorEssentials>> ExtractDescriptorsFromSingleAnalyzersLibrary(string assetGUID)
		{
			try
			{
				var assetPath = AssetDatabase.GUIDToAssetPath(assetGUID);
				var assetFullPath = Path.GetFullPath(assetPath);
				var analyzerJsonStream = StartProcessAndGetOutput(ExtractorPath, assetFullPath);
				var jsonSerializer = new JsonSerializer();
				var result = jsonSerializer.Deserialize<IEnumerable<DiagnosticDescriptorEssentials>>(analyzerJsonStream);
				return result?.Any() == true ? result : Enumerable.Empty<DiagnosticDescriptorEssentials>();
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
		private static JsonReader StartProcessAndGetOutput(string exePath, string arguments)
		{
			using var extractorProcess = new Process();
			extractorProcess.StartInfo.UseShellExecute = false;
			extractorProcess.StartInfo.RedirectStandardOutput = true;
			extractorProcess.StartInfo.FileName = exePath + ".exe";
			extractorProcess.StartInfo.Arguments = arguments;
			extractorProcess.StartInfo.CreateNoWindow = true;
			_ = extractorProcess.Start();
			return new JsonTextReader(extractorProcess.StandardOutput);
		}
#else
#endif
	}
}
