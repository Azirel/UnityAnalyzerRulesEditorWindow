using System;
using System.Collections.Generic;
using System.Linq;

namespace Azirel
{
	[Serializable]
	public readonly struct DiagnosticDescriptorEssentials
	{
		public readonly string Id;
		public readonly string Title;
		public readonly string Description;
		public readonly string HelpLinkUri;
		public readonly string Category;

		public DiagnosticDescriptorEssentials(dynamic diagnosticDescriptor)
		{
			Id = diagnosticDescriptor.Id;
			Title = diagnosticDescriptor.Title.ToString();
			Description = diagnosticDescriptor.Description.ToString();
			HelpLinkUri = diagnosticDescriptor.HelpLinkUri;
			Category = diagnosticDescriptor.Category;
		}

		public DiagnosticDescriptorEssentials(string id, string title, string description, string helpLinkUri, string category)
		{
			Id = id;
			Title = title;
			Description = description;
			HelpLinkUri = helpLinkUri;
			Category = category;
		}

		public static DiagnosticDescriptorEssentials Merge(IGrouping<string, DiagnosticDescriptorEssentials> descriptors)
		{
			if (descriptors is null)
				throw new ArgumentNullException(nameof(descriptors));
			if (!descriptors.Any())
				throw new ArgumentException("");
			var first = descriptors.First();
			return new(descriptors.Key, first.Title, JoinDescriptions(descriptors), first.HelpLinkUri, first.Category);
		}

		private static string JoinDescriptions(IEnumerable<DiagnosticDescriptorEssentials> descriptions)
		=> String.Join('\n', descriptions.Select(descriptor => descriptor.Description));
	}
}
