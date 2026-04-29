

using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AddressablesTracker
{
	private static readonly HashSet<AsyncOperationHandle> _sceneHandles = new();
	private static readonly HashSet<AsyncOperationHandle> _persistentHandles = new();

	public static void Track(AsyncOperationHandle h, bool isPersistent = false)
	{
		if (!h.IsValid()) return;
		
		if(isPersistent) _persistentHandles.Add(h);
		else _sceneHandles.Add(h);
	}

	public static void CleanUpTracker()
	{
		_sceneHandles.RemoveWhere(h => !h.IsValid());
		_persistentHandles.RemoveWhere(h => !h.IsValid());
	}

	public static void ReleaseSceneHandles()
	{
		CleanUpTracker();

		foreach (var h in _sceneHandles)
			if (h.IsValid()) Addressables.Release(h);
		_sceneHandles.Clear();
	}

	public static void ReleaseAll()
	{
		ReleaseSceneHandles();
		foreach (var h in _persistentHandles)
			if (h.IsValid()) Addressables.Release(h);
		_persistentHandles.Clear();
	}
}
