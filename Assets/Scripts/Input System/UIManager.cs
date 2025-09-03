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
	private string canvasTag = "UIRoot";

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
		Debug.Log("UI Tap");
	}

	public void OnDrag(Vector2 screenPos)
	{
		// UI 에서의 Drag 는 동작하지 않음
		// 예외로 사운드 조작바 같은건 있을 수 있을듯?
		return;
	}

	public void OnDragEnd(Vector2 screenPos)
	{
		// UI 에서의 DragEnd 는 동작하지 않음
		return;
	}

	public IEnumerator LoadUIForScene(Scenes scene)
	{
		var root = GameObject.FindWithTag(canvasTag);
		if (root == null) { yield return null; root = GameObject.FindWithTag(canvasTag); }
		if (root == null)
		{
			Debug.LogError($"UIManager : Canvas with tag '{canvasTag}' not found.");
			yield break;
		}

		var aref = sceneConfig.GetUIFor(scene);
		var handle = aref.InstantiateAsync();
		_currentHandle = handle;
		_hasHandle = true;

		yield return handle;

		if (handle.Status != AsyncOperationStatus.Succeeded)
		{
			Debug.LogError($"UIManager : Failed to Load UI Group : {handle.DebugName} / {handle.OperationException}");
			_hasHandle = false; _currentHandle = default;
			yield break;
		}

		currentUIGroup = handle.Result;
		if (currentUIGroup == null)
		{
			Debug.LogError("UIManager : Handle returned null GameObject");
			yield break;
		}

		currentUIGroup.transform.SetParent(root.transform, worldPositionStays: false);

		var uiList = currentUIGroup.GetComponentsInChildren<ISceneUI>(true);
		if (uiList == null || uiList.Length == 0)
		{
			Debug.LogError($"UIManager : No ISceneUI found in {scene}");
			yield break;
		}
		foreach (var ui in uiList) ui.InitUI();

		currentUIGroup.TryGetComponent(out currentUI);

		_loadingRoutine = null;
		Debug.Log("Load UI For Scene Complete");
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
				var h = _currentHandle;
				h.Completed += _ => { if (h.IsValid()) Addressables.ReleaseInstance(h); };
			}
			else Addressables.ReleaseInstance(_currentHandle);
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
