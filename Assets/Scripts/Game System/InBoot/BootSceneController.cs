using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

public class BootSceneController : MonoBehaviour
{
	public static BootSceneController Instance { get; private set; }

	[Header("Addressables Refs")]
	[SerializeField] private AssetReferenceT<EnemyData> enemyDataRef;
	[SerializeField] private AssetReferenceT<ItemDatabase> itemDBRef;
	[SerializeField] private AssetReferenceT<EnforceDatabase> enforceDBRef;

	[Header("Scenes")]
	[SerializeField] private SceneConfig sceneConfig;
	[SerializeField] private Scenes nextSceneName;

	private BootSceneUI _sceneUI;

	private readonly List<AsyncOperationHandle> _operations = new();

	private void Awake()
	{
		Instance = this;

		Application.targetFrameRate = 60;
		Screen.orientation = ScreenOrientation.Portrait;
		QualitySettings.SetQualityLevel(2);
		Time.fixedDeltaTime = 1f / 50f;
	}

	private void Start()
	{
		StartCoroutine(BootRoutine());
	}

	private void AddOp(AsyncOperationHandle op)
	{
		if (op.IsValid())
			_operations.Add(op);
		else
			Debug.LogError($"[Boot] Invalied handle : {op.DebugName} (Skip)");
	}

	private void AddLoadOp<T>(AssetReferenceT<T> aref) where T : UnityEngine.Object
	{
		if(aref == null || !aref.RuntimeKeyIsValid())
		{
			Debug.LogError($"[Boot] Missing or Invalid Addressable reference : {aref}");
			return;
		}
		var h = aref.LoadAssetAsync();
		_operations.Add(h);
	}

	private IEnumerator RunInitializeDataRoutine()
	{
		var handle = enforceDBRef.LoadAssetAsync();
		yield return handle;
		if(handle.Status == AsyncOperationStatus.Succeeded)
		{
			EnforceService.Initialize(handle.Result);
		}
		else
		{
			Debug.LogError($"Enforce DB Load Failed : {handle.OperationException}");
			yield break;
		}

		var task = SaveService.InitializeAsync();
		while (!task.IsCompleted)
			yield return null;

		if (task.IsFaulted)
		{
			Debug.LogError($"Data Init Failed : {task.Exception}");
			yield break;
		}
	}

	private IEnumerator BootRoutine()
	{
		/* Prefs&GameData Load */
		yield return RunInitializeDataRoutine();

		/* Managers Load */
		yield return ManagersInitializer.Instance.InitializeCommonManagers();

		/* UI Assets Load */
		yield return UIManager.Instance.LoadUIForScene(Scenes.BootScene);
		var curUI = UIManager.Instance.CurrentUI;
		if(curUI is not BootSceneUI bootUI || curUI == null) 
		{
			Debug.LogError($"curUI's Type : {curUI.GetType().Name} - Type Miss Erorr to 'BootSceneUI' ");
			yield break;
		}
		_sceneUI = bootUI;
		_sceneUI.UpdateText("Booting Start...");
		_sceneUI.UpdateProgress(0f);

		_operations.Clear();
		AddOp(Addressables.InitializeAsync());
		AddLoadOp(enemyDataRef);
		AddLoadOp(itemDBRef);

		foreach(var op in _operations)
		{
			if (!op.IsValid()) continue;
			op.Completed += h =>
			{
				if (!h.IsValid()) return;
				if (h.Status != AsyncOperationStatus.Succeeded)
					HandleError($"{h.DebugName} Data Load Failed");
			};
		}

		while(true)
		{
			bool allDone = true;
			float sum = 0f;
			int count = 0;

			foreach(var o in _operations)
			{
				if (!o.IsValid()) continue;
				if(!o.IsDone) allDone = false;
				sum += o.PercentComplete;
				count++;
			}

			float progress = (count > 0) ? (sum / count) : 1f;
			_sceneUI.UpdateProgress(progress);
			_sceneUI.UpdateText($"{(int)(progress * 100)}% : Loading...");

			if (allDone) break;
			yield return null;
		}

		_sceneUI.UpdateProgress(1f);
		_sceneUI.UpdateText($"Init Complite");

		foreach (var o in _operations)
			if (o.IsValid()) Addressables.Release(o);
		_operations.Clear();

		yield return ManagersInitializer.Instance.InitializeSceneManagers(nextSceneName);
		yield return sceneConfig.LoadSceneRoutine(nextSceneName);

		Destroy(gameObject);
	}

	private void HandleError(string msg)
	{
		StopAllCoroutines();
		_sceneUI.UpdateText(msg);

		if (_sceneUI is BootSceneUI bootUI)
			bootUI.ShowRetryBtn();
	}

	public void RestartBootRoutine()
	{
		StartCoroutine(BootRoutine());
	}
}
