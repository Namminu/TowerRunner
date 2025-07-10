using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
	[Serializable]
	public struct EnemyEntry
    {
		[Tooltip("Enemy Object Prefab")] 
		public AssetReferenceGameObject prefabRef;
		[Tooltip("Object Pool Size")] 
		public int poolSize;
		[Tooltip("Spawn Interval")] 
		public float spawnInterval;
	}

	[Header("Prefab & Pooling")]
	[Tooltip("All Enemy Object Pooling Datas")]
	public List<EnemyEntry> entries = new List<EnemyEntry>(); 
}
