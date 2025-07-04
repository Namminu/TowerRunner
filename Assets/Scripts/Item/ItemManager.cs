using System.Collections;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
	[SerializeField]
	private ItemDatabase itemDB;

	private void Start()
	{
		foreach (var entry in itemDB.entries)
			StartCoroutine(SpawnLoop(entry));
	}

	private IEnumerator SpawnLoop(ItemDatabase.ItemEntry entry)
	{
		while(true)
		{
			float baseInterval = entry.spawnInterval;
			float speedMul = GameSpeedManager.Instance.SpeedMultiplier;
			float difficulty = Mathf.Lerp(1f, 2f, (Mathf.Clamp01(Time.timeSinceLevelLoad / 120f)));
			float interval = (baseInterval / speedMul) * difficulty;

			yield return new WaitForSeconds(interval);

			Vector3 spawnPos = GetRandormSpawnPosition();
			ItemPoolingManager.Instance.Spawn(entry.data, spawnPos);
		}
	}

	private Vector3 GetRandormSpawnPosition()
	{
		float z = Mathf.Abs(Camera.main.transform.position.z);
		var leftTop = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, z));
		var rightTop = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, z));
		Vector3 pos = new(Random.Range(leftTop.x, rightTop.x), leftTop.y + 1f, 0);

		return pos;
	}
}
