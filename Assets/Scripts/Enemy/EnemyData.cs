using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
	[Serializable]
	public struct EnemyEntry
    {
		[Tooltip("Enemy Object Prefab")] 
		public BaseEnemy prefab;
		[Tooltip("Object Pool Size")] 
		public int poolSize;
		[Tooltip("Spawn Interval")] 
		public float spawnInterval;
	}

	[Header("Prefab & Pooling")]
	[Tooltip("All Enemy Object Pooling Datas")]
	public List<EnemyEntry> entries = new List<EnemyEntry>();

    [Header("Object moveDown Speed")]
    [Tooltip("Object Speed to Fall DownSide"), Range(1, 10)]                   
	public float moveDownSpeed = 2f;
}
