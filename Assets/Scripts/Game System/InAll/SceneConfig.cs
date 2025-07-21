using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[CreateAssetMenu(menuName = "Scriptable Objects/SceneConfig")]
public class SceneConfig : ScriptableObject
{
    [Serializable]
    public struct SceneEntry
    {
        public Scenes scene;
        public AssetReference sceneRef;
    }
    [Tooltip("Scene : Addressables Scene Ref Mapping")]
    public List<SceneEntry> entries = new List<SceneEntry>();


    /// <param name="scene">scene name in Scenes enum</param>
    /// <returns>Scene loaded in Addressables Assets</returns>
    public AssetReference GetSceneRef(Scenes scene)
    {
        foreach (var entry in entries)
        {
            if(entry.scene == scene)
                return entry.sceneRef;
        }
		Debug.LogError($"There's no {scene} Mapping in SceneConfig");
		return null;
	}

    public IEnumerator LoadSceneRoutine(Scenes scene)
    {
        var reference = GetSceneRef(scene);
        if (reference == null) yield break;

        var handle = reference.LoadSceneAsync();
        yield return handle;

        if (handle.Status != AsyncOperationStatus.Succeeded)
            Debug.LogError($"Scene Load Failed : {scene}");
    }
}
