using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public interface IInitializable
{
	void Init();
}

public class ManagersInitializer : MonoBehaviour
{
	[Header("Common Managers")]
	[SerializeField] private List<AssetReferenceGameObject> commonManagerRefs;

	[Header("Each Scene Managers")]
	[SerializeField] private AssetReferenceGameObject MainManagerRef;
	[SerializeField] private AssetReferenceGameObject TownManagerRef;
	[SerializeField] private AssetReferenceGameObject TowerManagerRef;

	private bool _isFirstInit;

	private void Awake()
	{
		_isFirstInit = false;
	}

	public IEnumerator InitializeCommonManagers()
	{
		if (_isFirstInit) yield break;

		_isFirstInit = true;
		foreach (var managerRef in commonManagerRefs)
			yield return LoadAndInitManager(managerRef, persistent: true);
	}

	public IEnumerator InitializeSceneManagers(Scenes scene)
	{

		var sceneRef = scene switch
		{
			Scenes.Main => MainManagerRef,
			Scenes.Town => TownManagerRef,
			Scenes.Tower => TowerManagerRef,
			_ => MainManagerRef
		};
		yield return LoadAndInitManager(sceneRef, persistent : false);
	}

	public IEnumerator LoadAndInitManager(
		AssetReferenceGameObject managerRef, bool persistent)
	{
		var handle = managerRef.InstantiateAsync();
		yield return handle;

		if (handle.Status != AsyncOperationStatus.Succeeded)
		{
			Debug.LogError($"{managerRef.RuntimeKey} Load Failed");
			yield break;
		}

		var root = handle.Result;
		if(persistent)
			DontDestroyOnLoad(root);

		foreach(var init in root.GetComponentsInChildren<IInitializable>(true))
			init.Init();
	}
}
