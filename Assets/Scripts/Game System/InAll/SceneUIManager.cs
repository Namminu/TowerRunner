using System.Collections;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SceneUIManager : MonoBehaviour, IInitializable
{
	public static SceneUIManager Instance { get; private set; }

	[SerializeField] private SceneUIConfig sceneConfig;
	[SerializeField] private string canvasTag = "UIRoot";

	private GameObject currentUIGroup;
	private Transform canvasRoot;
	private ISceneUI currentUI;
	public ISceneUI CurrentUI => currentUI;

	private void Awake()
	{
		if (Instance != null && Instance != this) { Destroy(gameObject); return; }
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public void Init()
	{
		
	}

	public IEnumerator LoadUIForScene(Scenes scene)
	{
		if (currentUIGroup != null) { Destroy(currentUIGroup); }

		if(canvasRoot == null)
		{
			var canvasGO = GameObject.FindWithTag(canvasTag);
			canvasRoot = canvasGO.transform;
		}

		var prefabRef = sceneConfig.GetUIFor(scene);
		var handle = prefabRef.InstantiateAsync();
		yield return handle;
		if(handle.Status != AsyncOperationStatus.Succeeded)
		{
			Debug.LogError($"UI Group Prefab Load Failed : {scene}");
			yield break;
		}

		currentUIGroup = handle.Result;
		currentUIGroup.transform.SetParent(canvasRoot, false);

		if (!currentUIGroup.TryGetComponent(out currentUI))
			Debug.LogError($"There's no ISceneUI Object In Scene : {scene}");

		currentUI?.InitUI();
	}
}
