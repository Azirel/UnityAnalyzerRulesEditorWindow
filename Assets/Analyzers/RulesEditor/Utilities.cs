using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Utilities
{
	public static ILookup<TKey, TElement> EmptyLookup<TKey, TElement>()
		=> Enumerable.Empty<KeyValuePair<TKey, TElement>>()
			.ToLookup(pair => pair.Key, pair => pair.Value);

	public static IDictionary<TKey, TElement> EmptyDictionary<TKey, TElement>()
		=> Enumerable.Empty<KeyValuePair<TKey, TElement>>()
		.ToDictionary(pair => pair.Key, elementSelector: pair => pair.Value);
}
