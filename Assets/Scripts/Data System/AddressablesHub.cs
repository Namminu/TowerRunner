using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Analytics;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AddressablesHub
{
    private class Entry
    {
        public AssetReference Aref;         // 원본 참조
        public AsyncOperationHandle Handle; // 보관 핸들
        public bool Resident;               // 상주 여부
    }

    private static readonly Dictionary<string, Entry> _cache = new();
    private static AsyncOperationHandle _initHandle;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void DomainReset()
    {
        _cache.Clear();
        _initHandle = default;
    }

    public static AsyncOperationHandle Initialize()
    {
        if (_initHandle.IsValid()) return _initHandle;
        _initHandle = Addressables.InitializeAsync();
        return _initHandle;
    }

    public static AsyncOperationHandle<T> LoadOnce<T>(AssetReferenceT<T> aref, bool resident = true) 
        where T : UnityEngine.Object
    {
        Initialize();

        var key = aref.RuntimeKey.ToString();
        if(_cache.TryGetValue(key, out var e))
        {
            return e.Handle.Convert<T>();
        }

        var handle = aref.LoadAssetAsync();
        _cache[key] = new Entry { Aref = aref, Handle = handle, Resident = resident };
        return handle;
	}

    public static AsyncOperationHandle<T> TryGetHandle<T>(AssetReferenceT<T> aref)
        where T : UnityEngine.Object
	{
        var key = aref.RuntimeKey.ToString();
        if( _cache.TryGetValue(key,out var e) && e.Handle.IsValid())
            return e.Handle.Convert<T>();
		return default;
    }

    public static void ReleaseTransients()
    {
        var toRemove = new List<string>();

        foreach (var (key, e) in _cache)
        {
            if (e.Resident) continue;

            if (e.Aref != null)
                e.Aref.ReleaseAsset();
            else if (e.Handle.IsValid())
                Addressables.Release(e.Handle);

            toRemove.Add(key);
        }
        foreach (var key in toRemove) _cache.Remove(key);
    }

    public static void ReleaseAll()
    {
        foreach(var e in _cache.Values)
        {
            if (e.Aref != null) e.Aref.ReleaseAsset();
            else if (e.Handle.IsValid()) Addressables.Release(e.Handle);
        }
        _cache.Clear();
    }

    public static bool IsLoaded<T>(AssetReferenceT<T> aref)
        where T : UnityEngine.Object
	{
        var key = aref.RuntimeKey.ToString();
        return _cache.TryGetValue(key, out var e) && e.Handle.IsValid();
    }

    public static T GetResultOrNull<T>(AssetReferenceT<T> aref)
        where T : UnityEngine.Object
	{
        var h = TryGetHandle(aref);
        return h.IsValid() && h.Status == AsyncOperationStatus.Succeeded ? h.Result : null;
    }
}
