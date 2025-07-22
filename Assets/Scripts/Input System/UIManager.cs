using System;
using System.Collections;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

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
		if (currentUIGroup != null) 
			Destroy(currentUIGroup);
		currentUI = null;

		if(!Enum.TryParse(scene.name, out Scenes sceneEnum))
		{
			Debug.LogError($"UI Manager : There's no {scene.name} in Scenes enum");
			return;
		}

		StartCoroutine(LoadUIForScene(sceneEnum));
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
		var handle = sceneConfig.GetUIFor(scene).InstantiateAsync();
		yield return handle;

		if(handle.Status != AsyncOperationStatus.Succeeded)
		{
			Debug.LogError($"UI Manager : Failed to Load UI Group : {handle.DebugName}");
			yield break;
		}

		currentUIGroup = handle.Result;
		currentUIGroup.transform.SetParent(
			GameObject.FindWithTag(canvasTag).transform, 
			worldPositionStays : false);

		if (!currentUIGroup.TryGetComponent(out currentUI))
		{
			Debug.LogError($"UI Manager : There's no ISceneUI Object");
			yield break;
		}
		currentUI.InitUI();
	}
}
