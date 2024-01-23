using System.Collections.Generic;
using System.Linq;

namespace Azirel
{
	public static class Utilities
	{
		public static ILookup<TKey, TElement> EmptyLookup<TKey, TElement>()
			=> Enumerable.Empty<KeyValuePair<TKey, TElement>>()
				.ToLookup(pair => pair.Key, pair => pair.Value);

		public static IDictionary<TKey, TElement> EmptyDictionary<TKey, TElement>()
			=> Enumerable.Empty<KeyValuePair<TKey, TElement>>()
			.ToDictionary(pair => pair.Key, elementSelector: pair => pair.Value);
	}
}