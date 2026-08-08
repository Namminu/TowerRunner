using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public static class AddressablesTracker
{
    private static readonly HashSet<AsyncOperationHandle> _sceneHandles = new();
    private static readonly HashSet<AsyncOperationHandle> _persistentHandles = new();
    private static AsyncOperationHandle _activeSceneHandle;
    private static AsyncOperationHandle _activeSceneManagerHandle;

    public static void TrackSceneHandle(AsyncOperationHandle<SceneInstance> h)
    {
        _activeSceneHandle = h;
        if (h.IsValid())
            _sceneHandles.Add(h);
    }

    public static void TrackSceneManagerHandle(AsyncOperationHandle h)
    {
        _activeSceneManagerHandle = h;
        if (h.IsValid())
            _sceneHandles.Add(h);
    }

    public static void Track(AsyncOperationHandle h, bool isPersistent = false)
    {
        if (!h.IsValid()) return;

        if (isPersistent) _persistentHandles.Add(h);
        else _sceneHandles.Add(h);
    }

    public static void UntrackSceneHandle(AsyncOperationHandle h)
    {
        if (_sceneHandles.Remove(h))
        {
        }
    }

    public static void CleanUpTracker()
    {
        _sceneHandles.RemoveWhere(h => !h.IsValid());
        _persistentHandles.RemoveWhere(h => !h.IsValid());
    }

    public static void ReleaseSceneHandles()
    {
        CleanUpTracker();

        var activeHandle = _activeSceneHandle;
        var activeManagerHandle = _activeSceneManagerHandle;
        var releaseHandles = new List<AsyncOperationHandle>(_sceneHandles.Count);

        foreach (var h in _sceneHandles)
        {
            if (activeHandle.Equals(h))
            {
                continue;
            }

            if (activeManagerHandle.Equals(h))
            {
                continue;
            }

            releaseHandles.Add(h);
        }

        foreach (var h in releaseHandles)
        {
            var id = h.GetHashCode();
            bool valid = h.IsValid();
            bool done = h.IsDone;
            string name = valid ? h.DebugName : "<invalid>";

            if (!valid)
            {
                continue;
            }

            var status = h.Status;
            Addressables.Release(h);
        }

        _sceneHandles.Clear();
        if (activeHandle.IsValid())
            _sceneHandles.Add(activeHandle);
    }

    public static void ReleaseAll()
    {
        ReleaseSceneHandles();
        foreach (var h in _persistentHandles)
            if (h.IsValid()) Addressables.Release(h);
        _persistentHandles.Clear();
    }
}
