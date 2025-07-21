using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour, IInitializable
{
	public static UIManager Instance { get; private set; }

	[SerializeField]
	private SceneUIConfig uiConfig;
	private GameObject currentUI;

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
		if(currentUI != null) Destroy(currentUI);

		var prefabRef = uiConfig.GetUIFor((Scenes)Enum.Parse(typeof(Scenes), scene.name));
		var handle = prefabRef.InstantiateAsync();
		handle.Completed += h =>
		{
			currentUI = h.Result;
			currentUI.transform.SetParent(transform, worldPositionStays: false);
		};
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
}
