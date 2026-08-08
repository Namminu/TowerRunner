using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour, IInitializable
{
	public static UIManager Instance { get; private set; }

	[SerializeField]
	private SceneUIConfig sceneConfig;
	[SerializeField]
	private GameObject uiRoot;

	private GameObject currentUIGroup;
	private ISceneUI currentUI;
	public ISceneUI CurrentUI => currentUI;

	private AsyncOperationHandle<GameObject> _currentHandle;
	private bool _hasHandle = false;
	private Coroutine _loadingRoutine;

	[SerializeField] private Image overlayImage;
	public Image OverlayImage => overlayImage;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		UnLoadCurrentUI();

		if(!Enum.TryParse(scene.name, out Scenes sceneEnum))
		{
			Debug.LogError($"UI Manager : There's no {scene.name} in Scenes enum");
			FirebaseManager.LogCrash($"{scene.name} not Founded in Scenes enum");
			return;
		}
		if (_loadingRoutine != null) StopCoroutine(_loadingRoutine);
		_loadingRoutine = StartCoroutine(LoadUIForScene(sceneEnum));
	}

	private void OnDestroy()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	public void Init()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	public void OnTap(Vector2 screenPos)
	{
		// UI ������ Tap �� �������� ����
		return;
	}

	public void OnDrag(Vector2 screenPos)
	{
		// UI ������ Drag �� �������� ����
		// ���ܷ� ���� ���۹� ������ ���� �� ������?
		return;
	}

	public void OnDragEnd(Vector2 screenPos)
	{
		// UI ������ DragEnd �� �������� ����
		return;
	}

	public IEnumerator LoadUIForScene(Scenes scene)
	{
		if (uiRoot == null) { yield return null; uiRoot = GameObject.FindWithTag("UIRoot"); }
		if (uiRoot == null)
		{
			Debug.LogError($"UIManager : Canvas with tag 'UIRoot' not found.");
			FirebaseManager.LogCrash($"UIManager : Canvas with tag 'UIRoot' not found");
			yield break;
		}

		var aref = sceneConfig.GetUIFor(scene);
		var handle = aref.InstantiateAsync();
		yield return handle;

		_currentHandle = handle;
		_hasHandle = true;

		if (handle.Status != AsyncOperationStatus.Succeeded)
		{
			Debug.LogError($"UIManager : Failed to Load UI Group : {handle.DebugName} / {handle.OperationException}");
			FirebaseManager.LogCrash($"{handle.DebugName} UI Load Failed");
			_hasHandle = false; _currentHandle = default;
			yield break;
		}

		AddressablesTracker.Track(_currentHandle, isPersistent: false);

		currentUIGroup = handle.Result;
		if (currentUIGroup == null)
		{
			Debug.LogError("UIManager : Handle returned null GameObject");
			FirebaseManager.LogCrash($"{handle.Result.name} return Null GameObject");
			yield break;
		}

		currentUIGroup.transform.SetParent(uiRoot.transform, worldPositionStays: false);

		var uiList = currentUIGroup.GetComponentsInChildren<ISceneUI>(true);
		if (uiList == null || uiList.Length == 0)
		{
			Debug.LogError($"UIManager : No ISceneUI found in {scene}");
			FirebaseManager.LogCrash($"{scene} Not Founded ISceneUI");
			yield break;
		}
		foreach (var ui in uiList) ui.InitUI();

		currentUIGroup.TryGetComponent(out currentUI);

		_loadingRoutine = null;
	}


	public void UnLoadCurrentUI()
	{
		if(_loadingRoutine != null)
		{
			StopCoroutine(_loadingRoutine);
			_loadingRoutine = null;
		}

		if(_hasHandle && _currentHandle.IsValid())
		{ 
			if(!_currentHandle.IsDone)
			{
				AddressablesTracker.UntrackSceneHandle(_currentHandle);
				var h = _currentHandle;
				h.Completed += _ =>
				{
					if (h.IsValid())
					{
						Addressables.ReleaseInstance(h);
					}
				};
			}
			else
			{
				AddressablesTracker.UntrackSceneHandle(_currentHandle);
				Addressables.ReleaseInstance(_currentHandle);
			}
		}
		else
		{
			if (currentUIGroup != null)
				Destroy(currentUIGroup);
		}

		_currentHandle = default;
		_hasHandle = false;
		currentUIGroup = null;
		currentUI = null;
	}
}
