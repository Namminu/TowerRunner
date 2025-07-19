using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyManager : MonoBehaviour
{
	public static EnemyManager Instance { get; private set; }

	//[SerializeField]
	//EnemyData enemyData;
	List<EnemyData.EnemyEntry> entries;

	private void Awake()
	{
		Instance = this;
		//AddressablesInitManager.OnInitialized += (enemyData, _) => SetData(enemyData);
	}

	public void SetData(EnemyData data)
	{
		entries = data.entries;
		foreach (var entry in entries)
			StartCoroutine(SpawnLoop(entry));
	}

	private IEnumerator SpawnLoop(EnemyData.EnemyEntry entry)
	{
		while(true)
		{
			float waitTime = entry.spawnInterval / GameSpeedManager.Instance.SpeedMultiplier;
			yield return new WaitForSeconds(waitTime);

			EnemyPoolingManager.Instance.Spawn(entry.prefabRef, GetSpawnPosition(), Quaternion.identity);
		}
	}

	private Vector3 GetSpawnPosition()
	{
		float z = Mathf.Abs(Camera.main.transform.position.z);
		var leftTop = Camera.main.ViewportToWorldPoint(new Vector3(0, 5, z));
		var rightTop = Camera.main.ViewportToWorldPoint(new Vector3(1, 5, z));
		Vector3 pos = new(Random.Range(leftTop.x, rightTop.x), leftTop.y + 1f, 0);

		return pos;
	}

	public void Init()
	{

	}
}
