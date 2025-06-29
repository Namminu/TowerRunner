using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyPoolingManager : MonoBehaviour
{
	public static EnemyPoolingManager Instance { get; private set; }

	[SerializeField]
	EnemyData enemyData;
	Dictionary<BaseEnemy, EnemyPool> pools;

	private void Awake()
	{
		pools = new Dictionary<BaseEnemy, EnemyPool>();
		foreach(var entry in enemyData.entries)
		{
			pools[entry.prefab] = 
				new EnemyPool(entry.prefab, entry.poolSize, transform);
		}
	}

	private void HandleOutofBounds(EnemyMover mover)
	{
		mover.OnOutofBounds -= HandleOutofBounds;
		Despawn(mover.GetComponent<BaseEnemy>());
	}


	public BaseEnemy Spawn(BaseEnemy prefab, Vector3 pos, Quaternion rot)
	{
		var enemy = pools[prefab].Spawn(pos, rot);
		var mover = enemy.GetComponent<EnemyMover>();
		mover.OnOutofBounds += HandleOutofBounds;
		return enemy;
	}

	public void Despawn(BaseEnemy instance)
		=> pools[instance.Prefab].Despawn(instance);

}