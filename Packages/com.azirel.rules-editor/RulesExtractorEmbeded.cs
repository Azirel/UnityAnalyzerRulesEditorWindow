using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;

namespace Azirel
{
	public static class RulesExtractorEmbeded
	{
		private static Type? diagnosticType;
		private const string DiagnosticTypeName = "Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer";
		private static string? dependenciesPath => Path.GetFullPath("./Packages/com.azirel.rules-editor/.DefaultDependencies");
		private const string AnalyzersFilter = "l:RoslynAnalyzer";

		static RulesExtractorEmbeded()
			=> AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;

		public static ILookup<string, AnalyzerRule> ExtractAllRules()
		{
			return AssetDatabase.FindAssets(AnalyzersFilter)
				.Select(guid => (guid, ExtractFromAsset(guid)))
				.Where(rules => rules.Item2?.Any() == true)
				.SelectMany(group => group.Item2,
				(group, descriptor) => (name: AssetNameFromGUID(group.guid), rule: new AnalyzerRule(descriptor, AssetNameFromGUID(group.guid))))
				.ToLookup(analyzerName => analyzerName.name, item => item.rule);
		}

		private static string AssetNameFromGUID(string guid)
		{
			var assetPath = AssetDatabase.GUIDToAssetPath(guid);
			return Path.GetFileNameWithoutExtension(assetPath);
		}

		private static IEnumerable<DiagnosticDescriptorEssentials> ExtractFromAsset(string analyzerLibraryGUID)
		{
			try
			{
				var assetPath = AssetDatabase.GUIDToAssetPath(analyzerLibraryGUID);
				var assetFullPath = Path.GetFullPath(assetPath);
				return Extract(assetFullPath);
			}
			catch (Exception e)
			{
				return Enumerable.Empty<DiagnosticDescriptorEssentials>();
			}
		}

		private static IEnumerable<DiagnosticDescriptorEssentials> Extract(string analyzerLibraryPath)
		{
			var mainAssembly = Assembly.LoadFrom(analyzerLibraryPath);
			var types = mainAssembly.GetTypes();
			diagnosticType = Array
				.Find(types, type => IsSubClassOf(type, DiagnosticTypeName))?
				.BaseType;
			return diagnosticType is not null
				? mainAssembly.GetTypes()
						.Where(IsDiagnosticAnalyzer)
						.SelectMany(GetDescriptorsFromAnalyzerType)
						.Select(descriptor => new DiagnosticDescriptorEssentials(descriptor))
						.GroupBy(descriptor => descriptor.Id)
						.Select(group => DiagnosticDescriptorEssentials.Merge(group))
						.OrderBy(item => item.Id)
						.ToList()
				: Enumerable.Empty<DiagnosticDescriptorEssentials>();
		}

		private static IEnumerable<dynamic>? GetDescriptorsFromAnalyzerType(Type type)
		{
			dynamic analyzerInstance = Activator.CreateInstance(type)!;
			return analyzerInstance is not null ? (analyzerInstance.SupportedDiagnostics as IEnumerable<dynamic>) : throw new Exception($"{nameof(analyzerInstance)} is null");
		}

		private static bool IsDiagnosticAnalyzer(Type type)
			=> type?.IsSubclassOf(diagnosticType!) == true && !type.IsAbstract;

		private static Assembly? ResolveAssembly(object? sender, ResolveEventArgs args)
		{
			const string dependencyNamePattern = "^([^,]+)";
			var dependencyName = Regex.Match(args.Name, dependencyNamePattern).Value;
			var assemblyPath = Array.Find(Directory.GetFiles(dependenciesPath), path => Path.GetFileNameWithoutExtension(path) == dependencyName);
			return String.IsNullOrEmpty(assemblyPath) ? null : Assembly.LoadFrom(assemblyPath);
		}

		private static bool IsSubClassOf(Type mainType, string targetClassFullName) => mainType?.BaseType is not null
		&& ((mainType.BaseType.FullName == targetClassFullName
						&& !mainType.IsAbstract)
			|| IsSubClassOf(mainType.BaseType, targetClassFullName));
	}
}