

using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AddressablesTracker
{
	private static readonly List<AsyncOperationHandle> _handles = new();

	public static void Track(AsyncOperationHandle h)
	{
		if(h.IsValid()) _handles.Add(h);
	}

	public static void ReleaseAll()
	{
		foreach (var h in _handles)
			if (h.IsValid()) Addressables.Release(h);
		_handles.Clear();
	}
}
