using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public enum Scenes
{
	BootScene,
	Main,
	Town,
	Tower
}

class Operation
{
	public AsyncOperationHandle Handle;
	public string Description;
	public Operation(AsyncOperationHandle handle, string desc)
	{
		Handle = handle;
		Description = desc;
	}
}

public class BootSceneController : MonoBehaviour
{
	[Header("Addressables Refs")]
	[SerializeField] private AssetReferenceT<EnemyData> enemyDataRef;
	[SerializeField] private AssetReferenceT<ItemDatabase> itemDBRef;
	[SerializeField] private Scenes nextSceneName;

	[Header("Each Scenes Manager Prefab Refs")]
	[SerializeField] private AssetReferenceGameObject MainManagerRef;
	[SerializeField] private AssetReferenceGameObject TownManagerRef;
	[SerializeField] private AssetReferenceGameObject TowerManagerRef;

	[SerializeField] private AssetReferenceGameObject inputManagerRef;
	[SerializeField] private AssetReferenceGameObject gameSpeedManagerRef;
	[SerializeField] private AssetReferenceGameObject poolingManagerRef;
	[SerializeField] private AssetReferenceGameObject objectManagerRef;
	[SerializeField] private AssetReferenceGameObject audioManagerRef;
	[SerializeField] private AssetReferenceGameObject uiManagerRef;

	[Header("UI Components")]
	[SerializeField] private Slider progressBar;
	[SerializeField] private Text initStateText;
	[SerializeField] private Button retryBtn;

	private readonly List<Operation> _operations = new();

	private void Awake()
	{
		Application.targetFrameRate = 60;
		Screen.orientation = ScreenOrientation.Portrait;
		QualitySettings.SetQualityLevel(2);
		Time.fixedDeltaTime = 1f / 50f;
	}

	private void OnEnable()
	{
		retryBtn.gameObject.SetActive(false);
		retryBtn.onClick.AddListener(() =>
		{
			retryBtn.gameObject.SetActive(false);
			StartCoroutine(BootRoutine());
		});

		StartCoroutine(BootRoutine());
	}

	private IEnumerator BootRoutine()
	{
		progressBar.value = 0f;
		initStateText.text = "Boot Start...";

		yield return InitializeManager(GetManagersRefFor(nextSceneName), root =>
		{
			foreach (var init in root.GetComponentsInChildren<IInitializable>(true))
				init.Init();
		}, "Managers Init Failed");


		//yield return InitializeManager(inputManagerRef, go => 
		//	go.GetComponent<InputManger>()?.Init(), "InputManager Init Failed");
		//yield return InitializeManager(gameSpeedManagerRef, go => 
		//	go.GetComponent<GameSpeedManager>()?.Init(), "GameSpeedManager Init Failed");
		//yield return InitializeManager(poolingManagerRef, go => {
		//	go.GetComponentInChildren<EnemyPoolingManager>()?.Init();
		//	go.GetComponentInChildren<ItemPoolingManager>()?.Init();
		//}, "PoolingManager Init Failed");
		//yield return InitializeManager(objectManagerRef, go => {
		//	go.GetComponentInChildren<EnemyManager>()?.Init();
		//	go.GetComponentInChildren<ItemManager>()?.Init();
		//}, "ObjectManager Init Failed");
		////yield return InitializeManager(audioManagerRef, go =>
		////	go.GetComponent<AudioManager>()?.Init(), "AudioManager Init Failed");
		//yield return InitializeManager(uiManagerRef, go => 
		//	go.GetComponent<UIManager>()?.Init(), "UIManager Init Failed");

		RegisterOperation(
			Addressables.InitializeAsync(),
			"Addressables Initializing...",
			"Addressables Init Failed"
		);
		RegisterOperation(
			enemyDataRef.LoadAssetAsync(),
			"EnemyData Loading...",
			"EnemyData Load Failed"
		);
		RegisterOperation(
			itemDBRef.LoadAssetAsync(),
			"ItemDatabase Loading...",
			"ItemDatabase Load Failed"
		);

		while (!_operations.TrueForAll(op => op.Handle.IsDone))
		{
			float sum = 0f;
			foreach (var op in _operations) sum += op.Handle.PercentComplete;
			float progress = sum / _operations.Count;
			progressBar.value = progress;

			var current = _operations.Find(op => !op.Handle.IsDone);
			initStateText.text = $"{(int)progress * 100f}% : {current.Description}";

			yield return null;
		}

		progressBar.value = 1f;
		initStateText.text = "Init Complete";

		foreach (var op in _operations)
			Addressables.Release(op.Handle);
		_operations.Clear();

		Addressables.LoadSceneAsync(nextSceneName.ToString());

		Destroy(gameObject);
	}

	private AssetReferenceGameObject GetManagersRefFor(Scenes scene)
	{
		return scene switch
		{
			Scenes.Tower => TowerManagerRef,
			Scenes.Main => MainManagerRef,
			Scenes.Town => TownManagerRef,
			_ => TowerManagerRef
		};
	}

	private IEnumerator InitializeManager(
		AssetReferenceGameObject prefabRef,
		Action<GameObject> initAction,
		string errorMsg)
	{
		progressBar.value = 0f;
		initStateText.text = $"Loading {prefabRef.AssetGUID}...";
		var handle = prefabRef.InstantiateAsync();
		_operations.Add(new Operation(handle, $"Instantiate {prefabRef.RuntimeKey}"));
		handle.Completed += h =>
		{
			if (h.Status == AsyncOperationStatus.Succeeded)
				initAction(h.Result);
			else
				HandleError(errorMsg);
		};

		yield return handle;
	}

	private void RegisterOperation(
		AsyncOperationHandle handle,
		string desc,
		string errorMsg)
	{
		var op = new Operation(handle, desc);
		_operations.Add(op);
		handle.Completed += h =>
		{
			if (h.Status != AsyncOperationStatus.Succeeded)
				HandleError(errorMsg);
		};
	}

	private void HandleError(string msg)
	{
		StopAllCoroutines();
		initStateText.text = msg;
		retryBtn.gameObject.SetActive(true);

		foreach (var op in _operations)
			Addressables.Release(op.Handle);
		_operations.Clear();
	}
}
