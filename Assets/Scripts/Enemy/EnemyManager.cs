using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
	[SerializeField]
	EnemyData enemyData;

	// Dictionary<BaseEnemy, float> timers = new Dictionary<BaseEnemy, float>();

	private void Start()
	{
		foreach (var entry in enemyData.entries)
			StartCoroutine(SpawnLoop(entry));
	}

	private IEnumerator SpawnLoop(EnemyData.EnemyEntry entry)
	{
		while(true)
		{
			float speedMul = GameSpeedManager.Instance.SpeedMultiplier;
			float waitTime = entry.spawnInterval * speedMul;
			yield return new WaitForSeconds(waitTime);

			Spawn(entry.prefab);
		}
	}

	private void Spawn(BaseEnemy prefab)
	{
		Vector3 pos = GetSpawnPosition();
		EnemyPoolingManager.Instance.Spawn(prefab, pos, Quaternion.identity);
	}

	private Vector3 GetSpawnPosition()
	{
		float z = Mathf.Abs(Camera.main.transform.position.z);
		var leftTop = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, z));
		var rightTop = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, z));
		Vector3 pos = new Vector3(Random.Range(leftTop.x, rightTop.x), leftTop.y + 1f, 0);

		return pos;
	}
}
