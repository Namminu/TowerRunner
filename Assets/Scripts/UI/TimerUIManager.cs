using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class TimerUIManager : MonoBehaviour, IInitializable
{
	public static TimerUIManager Instance { get; private set; }

	[Header("Addressables Setting")]
	[SerializeField] private AssetReferenceGameObject timerPrefab;
	[SerializeField, Tooltip("Item Count need Timer UI")]
	private int totalTimerCount = 3;

	private Transform entryParent;

	private ObjectPool<TimerUIEntry> timerPool;
	private AsyncOperationHandle<GameObject> loadHandle;

	private void Awake()
	{
		//Instance = this;

		//timerPool = new ObjectPool<TimerUIEntry>(timerPrefab, initialSize: totalTimerCount, parent: transform);
	}

	public void Init()
	{
		Instance = this;

		LoadAndInitPool();
	}

	private void LoadAndInitPool()
	{
		loadHandle = timerPrefab.LoadAssetAsync<GameObject>();

		loadHandle.Completed += (handle) =>
		{
			if (handle.Status == AsyncOperationStatus.Succeeded)
			{
				TimerUIEntry prefabComponent = handle.Result.GetComponent<TimerUIEntry>();
				if (prefabComponent == null)
				{
					Debug.Log("Can not Find Timer UI Entry in Buff Icon Asset");
					return;
				}
				timerPool = new ObjectPool<TimerUIEntry>(prefabComponent, totalTimerCount, transform);
			}
			else
			{
				Debug.LogError("TimerUIEntry Load Failed!");
			}
		};
	}

	public void StartTimer(Sprite icon, float duration, System.Action onComplete)
	{
		//var entry = timerPool.Spawn();
		//entry.transform.SetAsLastSibling();
		//entry.Setup(Icon, duration, () =>
		//{
		//	onComplete?.Invoke();
		//	timerPool.Despawn(entry);
		//});

		var entry = timerPool.Spawn();

		if (entryParent != null)
		{
			entry.transform.SetParent(entryParent, false);
		}
		else Debug.Log("Entry Parent Null Error");

		entry.transform.SetAsLastSibling();
		entry.Setup(icon, duration, () =>
		{
			onComplete?.Invoke();
			timerPool.Despawn(entry);
		});
	}

	private void OnDestroy()
	{
		Instance = null;

		if (loadHandle.IsValid())
		{
			Addressables.Release(loadHandle);
		}
	}

	public void RegisterParent(Transform entry) => entryParent = entry;
}
