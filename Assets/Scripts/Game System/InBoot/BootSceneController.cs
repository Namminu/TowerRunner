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
	[Header("Initializer")]
	[SerializeField] private ManagersInitializer managersInitializer;

	[Header("Addressables Refs")]
	[SerializeField] private AssetReferenceT<EnemyData> enemyDataRef;
	[SerializeField] private AssetReferenceT<ItemDatabase> itemDBRef;

	[Header("UI Components")]
	[SerializeField] private Slider progressBar;
	[SerializeField] private Text initStateText;
	[SerializeField] private Button retryBtn;

	[Header("Scene")]
	[SerializeField] private Scenes nextSceneName;

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
		retryBtn.onClick.RemoveAllListeners();
		retryBtn.onClick.AddListener(() =>
		{
			retryBtn.gameObject.SetActive(false);
			StartCoroutine(BootRoutine());
		});

		retryBtn.gameObject.SetActive(false);
		StartCoroutine(BootRoutine());
	}

	private IEnumerator BootRoutine()
	{
		progressBar.value = 0f;
		initStateText.text = "Boot Start...";

		initStateText.text = "Common Managers Initializing...";
		yield return managersInitializer.InitializeCommonManagers();

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

		initStateText.text = $"Loading Next Scene to Game Start...";
		var nextScene = Addressables.LoadSceneAsync(nextSceneName.ToString());
		yield return nextScene;

		if(nextScene.Status != AsyncOperationStatus.Succeeded)
		{
			HandleError($"Failed to Load Scene : {nextSceneName}");
			yield break;
		}
		initStateText.text = $"Loading Next Scene to Game Start...";
		yield return managersInitializer.InitializeSceneManagers(nextSceneName);

		Destroy(gameObject);
	}

	private void RegisterOperation(
		AsyncOperationHandle handle,
		string desc, string errorMsg)
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
