using UnityEngine;

public class TimerUIManager : MonoBehaviour
{
	public static TimerUIManager Instance { get; private set; }

	[SerializeField]
	private TimerUIEntry timerPrefab;
	[SerializeField, Tooltip("Layout Group Component Parent")]
	private Transform entryParent;

	private ObjectPool<TimerUIEntry> timerPool;

	[SerializeField, Tooltip("Item Count need Timer UI")]
	private int totalTimerCount;

	private void Awake()
	{
		Instance = this;
		timerPool = new ObjectPool<TimerUIEntry>(timerPrefab, initialSize: totalTimerCount, parent: transform);
	}

	public void StartTimer(Sprite Icon, float duration, System.Action onComplete)
	{
		var entry = timerPool.Spawn();
		entry.transform.SetAsLastSibling();
		entry.Setup(Icon, duration, () =>
		{
			onComplete?.Invoke();
			timerPool.Despawn(entry);
		});
	}
}
