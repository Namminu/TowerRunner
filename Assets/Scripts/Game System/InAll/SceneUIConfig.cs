using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Scriptable Objects/SceneUIConfig")]
public class SceneUIConfig : ScriptableObject
{
    [Serializable]
    public struct UIEntry
    {
        public Scenes scene;
        public AssetReferenceGameObject uiPrefab;
    }
    public List<UIEntry> entries = new();

    public AssetReferenceGameObject GetUIFor(Scenes scene)
    {
        var entry = entries.Find(x => x.scene == scene);
        return entry.uiPrefab;
    }
}
