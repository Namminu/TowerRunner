using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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

	[Header("Config")]
	[SerializeField] private BootConfig config;

	[Header("Scenes")]
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

	//private void Add(AsyncOperationHandle h)
	//{
	//	if (!h.IsValid())
	//		Debug.LogError($"[Boot] Invalid Handle {h.DebugName} Skipped");
	//	_operations.Add(h);
	//	AddressablesTracker.Track(h);
	//}

	//private void AddOp(AsyncOperationHandle op)
	//{
	//	if (op.IsValid())
	//		_operations.Add(op);
	//	else
	//		Debug.LogError($"[Boot] Invalied handle : {op.DebugName} (Skip)");
	//}

	//private void AddLoadOp<T>(AssetReferenceT<T> aref) where T : UnityEngine.Object
	//{
	//	if(aref == null || !aref.RuntimeKeyIsValid())
	//	{
	//		Debug.LogError($"[Boot] Missing or Invalid Addressable reference : {aref}");
	//		return;
	//	}
	//	var h = aref.LoadAssetAsync();
	//	_operations.Add(h);
	//}

	private IEnumerator RunInitializeDataRoutine()
	{
		var task = SaveService.InitializeAsync();
		while (!task.IsCompleted) yield return null;
	}

	private IEnumerator BootRoutine()
	{
		/* Prefs & GameData Load */
		yield return RunInitializeDataRoutine();

		/* Required Reference Verification */
		if(!ValidateConfig(out var reason))
		{
			Fail($"Essential Addressables Missing : {reason}");
			yield break;
		}

		/* Managers Load */
		yield return ManagersInitializer.Instance.InitializeCommonManagers();

		/* UI Assets Load */
		yield return UIManager.Instance.LoadUIForScene(Scenes.BootScene);
		_sceneUI = UIManager.Instance.CurrentUI as BootSceneUI;
		if (!_sceneUI)
		{
			Debug.LogError("Boot UI not Found");
			yield break;
		}
		_sceneUI.UpdateText("Init Complete");
		_sceneUI.UpdateProgress(1f);

		/* Load with Once Self Recovery */
		var success = false;
		for (int attempt = 0; attempt <= 1 && !success; attempt++)
		{
			_operations.Clear();

			var initH = AddressablesHub.Initialize();
			var itemDBH = AddressablesHub.LoadOnce(config.itemDBRef, resident: true);
			var enforceH = AddressablesHub.LoadOnce(config.enforceDBRef, resident: true);
			var enemyH = AddressablesHub.LoadOnce(config.enemyDataRef, resident: false);

			_operations.Add(initH);
			_operations.Add(itemDBH);
			_operations.Add(enforceH);
			_operations.Add(enemyH);

			/* Detect Failed Case */
			foreach (var op in _operations)
			{
				if (!op.IsValid()) continue;
				op.Completed += h =>
				{
					if (h.IsValid() && h.Status != AsyncOperationStatus.Succeeded)
						Debug.LogError($"[Boot] {h.DebugName} Failed : {h.OperationException}");
				};
			}

			/* Update Progress Bar */
			yield return TrackProgress();

			/* Result */
			success = AllSucceeded();

			/* Release Handle which Resident is True */
			AddressablesHub.ReleaseTransients();

			if (!success && attempt == 0)
				yield return TrySelfHeal();
		}

		if (!success)
		{
			Fail("Boot Essential Data Load Failed");
			yield break;
		}

		/* Services Init After Load Success */
		var itemDB = AddressablesHub.GetResultOrNull(config.itemDBRef);
		var enforce = AddressablesHub.GetResultOrNull(config.enforceDBRef);

		if(itemDB == null || enforce == null)
		{
			Fail("DB Resolve Failed");
			yield break;
		}

		EnforceService.Initialize(enforce);
		InventoryService.Initialize(itemDB);

		InventoryService.SyncFromSave();

		_sceneUI.UpdateText("Init Complete");
		_sceneUI.UpdateProgress(1f);

		/* Load Next Scene */
		yield return ManagersInitializer.Instance.SceneLoadRoutine(nextSceneName);
		Destroy(gameObject);
	}

	private IEnumerator TrackProgress()
	{
		while(true)
		{
			bool allDone = true;
			float sum = 0f;
			int count = 0;

			foreach(var o in _operations)
			{
				if (!o.IsValid())
				{
					allDone = false;
					continue;
				}
				if (!o.IsDone)
					allDone = false;

				sum += o.PercentComplete;
				count++;
			}
			_sceneUI.UpdateProgress(count > 0 ? sum / count : 0f);
			_sceneUI.UpdateText($"{(int)((count > 0 ? sum/count : 0f) * 100)}% : Loading...");
			if (allDone) break;
			yield return null;
		}
	}

	private bool AllSucceeded()
	{
		foreach(var o in _operations)
		{
			if (!o.IsValid()) return false;
			if(o.Status != AsyncOperationStatus.Succeeded) return false;
		}
		return true;
	}

	private bool ValidateConfig(out string reason)
	{
		reason = "";
		if(config == null) { reason = "BootConfig"; return false; }
		if(config.enemyDataRef == null || !config.enemyDataRef.RuntimeKeyIsValid())
			reason += " enemyDataRef";
		if (config.itemDBRef == null || !config.itemDBRef.RuntimeKeyIsValid())
			reason += " itemDBRef";
		if (config.enforceDBRef == null || !config.enforceDBRef.RuntimeKeyIsValid())
			reason += " enforceDBRef";
		return string.IsNullOrEmpty(reason);
	}

	private IEnumerator TrySelfHeal()
	{
		var check = Addressables.CheckForCatalogUpdates(false);
		yield return check; 

		if(check.Status == AsyncOperationStatus.Succeeded && 
			check.Result != null && 
			check.Result.Count > 0)
		{
			var update = Addressables.UpdateCatalogs(check.Result);
			yield return update;
		}
		else
		{
			var init = Addressables.InitializeAsync();
			if (!init.IsDone) yield return init;
		}
	}

	private void Fail(string msg)
	{
		Debug.LogError($"[Boot Fatal] : {msg}");
		_sceneUI.UpdateText(msg);
		_sceneUI.UpdateProgress(0f);
	}

	public void RestartBootRoutine()
		=> StartCoroutine(BootRoutine());
}
