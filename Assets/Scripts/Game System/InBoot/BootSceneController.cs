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

	private IEnumerator BootRoutine()
	{
		yield return ManagersInitializer.Instance.InitializeCommonManagers();

		yield return SceneUIManager.Instance.LoadUIForScene(Scenes.BootScene);

		var curUI = SceneUIManager.Instance.CurrentUI;
		if(curUI is not BootSceneUI bootUI) 
		{
			Debug.LogError($"curUI's Type : {curUI.GetType().Name} - Type Miss Erorr to 'BootSceneUI' ");
			yield break;
		}
		_sceneUI = bootUI;

		_sceneUI.UpdateText("Booting Start...");
		_sceneUI.UpdateProgress(0f);

		_operations.Clear();
		_operations.Add(Addressables.InitializeAsync());
		_operations.Add(enemyDataRef.LoadAssetAsync());
		_operations.Add(itemDBRef.LoadAssetAsync());

		foreach(var op in _operations)
		{
			op.Completed += h =>
			{
				if (h.Status != AsyncOperationStatus.Succeeded)
					HandleError($"{h.DebugName} Data Load Failed");
			};
		}

		while(!_operations.TrueForAll(o => o.IsDone))
		{
			float sum = 0f;
			_operations.ForEach(o => sum += o.PercentComplete);
			float progress = sum / _operations.Count;
			_sceneUI.UpdateProgress(progress);
			_sceneUI.UpdateText($"{(int)(progress * 100)}% : Loading...");
			yield return null;
		}

		_sceneUI.UpdateProgress(1f);
		_sceneUI.UpdateText($"Init Complite");

		_operations.ForEach(o => Addressables.Release(o));
		_operations.Clear();

		yield return ManagersInitializer.Instance.InitializeSceneManagers(nextSceneName);
		yield return SceneUIManager.Instance.LoadUIForScene(nextSceneName);
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

	public  void RestartBootRoutine()
	{
		StartCoroutine(BootRoutine());
	}
}
