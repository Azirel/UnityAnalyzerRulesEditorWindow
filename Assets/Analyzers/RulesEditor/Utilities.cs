using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Utilities
{
	public static ILookup<TKey, TElement> Empty<TKey, TElement>()
		=> Enumerable.Empty<KeyValuePair<TKey, TElement>>()
			.ToLookup(pair => pair.Key, pair => pair.Value);
}
