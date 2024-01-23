using System;

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

		public DiagnosticDescriptorEssentials(string id, string title, string description, string helpLinkUri, string category)
		{
			Id = id;
			Title = title;
			Description = description;
			HelpLinkUri = helpLinkUri;
			Category = category;
		}
	}
}