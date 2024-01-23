using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Azirel
{
	public class AnalyzerRule : ISearchable
	{
		public readonly string Id;
		public readonly DiagnosticDescriptorEssentials Descriptor;
		public readonly string AnalyzerId;
		private DiagnosticSeverity severity = DiagnosticSeverity.Hidden;

		public DiagnosticSeverity Severity
		{
			get => severity;
			set
			{
				if (severity != value)
				{
					severity = value;
					SeverityValueChange?.Invoke(severity);
				}
			}
		}

		public event Action<DiagnosticSeverity> SeverityValueChange;

		private IEnumerable<string> GetSearchableContent()
		{
			yield return Id;
			yield return AnalyzerId;
			yield return Descriptor.Title;
			yield return Descriptor.Category;
			yield return Descriptor.HelpLinkUri;
			yield return Descriptor.Description;
		}

		public AnalyzerRule(string id, string analyzerId, DiagnosticSeverity severity = DiagnosticSeverity.None)
		{
			Id = id;
			Severity = severity;
			AnalyzerId = analyzerId;
		}

		public AnalyzerRule(DiagnosticDescriptorEssentials descriptor, string analyzerId) : this(id: descriptor.Id, analyzerId: analyzerId)
			=> Descriptor = descriptor;

		[JsonConstructor]
		public AnalyzerRule(DiagnosticDescriptorEssentials descriptor, string analyzerId, DiagnosticSeverity severity) : this(descriptor, analyzerId)
			=> Severity = severity;

		public bool Match(string query)
			=> String.IsNullOrEmpty(query)
			|| GetSearchableContent().
			Any(@string => @string.Contains(query));
	}
}