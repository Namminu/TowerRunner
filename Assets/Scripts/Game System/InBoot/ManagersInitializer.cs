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
	private bool _isSceneLoading;
	private GameObject _currentSceneManager;
	private AsyncOperationHandle<GameObject> _currentSceneManagerHandle;
	private static int _sceneLoadSeq = 0;

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
		if (_isSceneLoading)
		{
			Debug.LogWarning($"SceneLoad request blocked: already loading {nextScene}");
			return;
		}

		_isSceneLoading = true;
		_sceneLoadSeq++;
		StartCoroutine(SceneLoadRoutine(nextScene));
	}

	private IEnumerator SceneLoadRoutine(Scenes scene)
	{
		try
		{
			yield return InitializeSceneManagers(scene);
			yield return sceneConfig.LoadSceneRoutine(scene);
		}
		finally
		{
			_isSceneLoading = false;
		}
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
		if (!persistent)
			AddressablesTracker.TrackSceneManagerHandle(handle);

		var root = handle.Result;
		if (persistent)
			DontDestroyOnLoad(root);
		else
		{
			if(_currentSceneManager != null)
			{
				var previousHandle = _currentSceneManagerHandle;
				var previousManager = _currentSceneManager;
				AddressablesTracker.UntrackSceneHandle(previousHandle);
				Addressables.ReleaseInstance(previousManager);
				_currentSceneManager = null;
				_currentSceneManagerHandle = default;
				yield return null;
			}
			_currentSceneManager = root;
			_currentSceneManagerHandle = handle;
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

		SceneLoad(Scenes.Main); // Main ���� ���� ���� ���� üũ
	}
}

