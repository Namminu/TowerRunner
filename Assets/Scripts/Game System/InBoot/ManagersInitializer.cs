using Firebase.Analytics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public interface IInitializable
{
	void Init();
}

public class ManagersInitializer : MonoBehaviour
{
	public static ManagersInitializer Instance { get; private set; }

	[Header("Common Managers")]
	[SerializeField] private List<AssetReferenceGameObject> commonManagerRefs;

	[Header("Each Scene Managers")]
	[SerializeField] private AssetReferenceGameObject MainManagerRef;
	[SerializeField] private AssetReferenceGameObject TownManagerRef;
	[SerializeField] private AssetReferenceGameObject TowerManagerRef;
	[SerializeField] private AssetReferenceGameObject LoadingManagerRef;

	[Header("Scene Config")]
	[SerializeField] private SceneConfig sceneConfig;

	private bool _isFirstInit;
	private GameObject _currentSceneManager;

	private void Awake()
	{
		_isFirstInit = false;
		if(Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public IEnumerator InitializeCommonManagers()
	{
		if (_isFirstInit) yield break;

		_isFirstInit = true;
		foreach (var managerRef in commonManagerRefs)
			yield return LoadAndInitManager(managerRef, persistent: true);
	}

	public void SceneLoad(Scenes nextScene)
	{
		StartCoroutine(SceneLoadRoutine(nextScene));
	}

	private IEnumerator SceneLoadRoutine(Scenes scene)
	{
		yield return InitializeSceneManagers(scene);
		yield return sceneConfig.LoadSceneRoutine(scene);
	}

	private IEnumerator InitializeSceneManagers(Scenes scene)
	{
		var sceneRef = scene switch
		{
			Scenes.Main => MainManagerRef,
			Scenes.Town => TownManagerRef,
			Scenes.Tower => TowerManagerRef,
			Scenes.Loading => LoadingManagerRef,
			_ => MainManagerRef
		};
		yield return LoadAndInitManager(sceneRef, persistent: false);
	}

	private IEnumerator LoadAndInitManager(
		AssetReferenceGameObject managerRef, bool persistent)
	{
		var handle = managerRef.InstantiateAsync();
		yield return handle;

		if (handle.Status != AsyncOperationStatus.Succeeded)
		{
			Debug.LogError($"{managerRef.RuntimeKey} Load Failed");
			FirebaseManager.LogCrash($"{handle.DebugName} Load Failed");
			yield break;
		}

		AddressablesTracker.Track(handle, isPersistent: persistent);

		var root = handle.Result;
		if (persistent)
			DontDestroyOnLoad(root);
		else
		{
			if(_currentSceneManager != null)
			{
				Addressables.ReleaseInstance(_currentSceneManager);
				_currentSceneManager = null;
				yield return null;
			}
			_currentSceneManager = root;
			root.transform.parent = transform;
		}

		root.GetComponent<IInitializable>()?.Init();
	}

	public void ResetCall()
	{
		StartCoroutine(GameResetRoutine());
	}

	private IEnumerator GameResetRoutine()
	{
		SaveService.ResetSaveData();

		UIManager.Instance.UnLoadCurrentUI();
		AddressablesTracker.ReleaseSceneHandles();

		ScoreManager.Instance.ResetScore();
		InventoryService.SyncFromSave();

		yield return null;

		SceneLoad(Scenes.Main); // Main 으로 변경 가능 여부 체크
	}
}

