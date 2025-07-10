using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class EnemyPoolingManager : MonoBehaviour
{
	public static EnemyPoolingManager Instance { get; private set; }

	[SerializeField]
	EnemyData enemyData;
	Dictionary<AssetReferenceGameObject, EnemyPool> pools;

	private void Awake()
	{
		Instance = this;
		pools = new Dictionary<AssetReferenceGameObject, EnemyPool>();
	}

	private void Start()
	{
		foreach (var entry in enemyData.entries)
		{
			pools[entry.prefabRef] =
				new EnemyPool(entry.prefabRef, entry.poolSize, transform);
		}
	}

	private void HandleOutofBounds(ObjectMover mover)
	{
		mover.OnOutofBounds -= HandleOutofBounds;
		Despawn(mover.GetComponent<BaseEnemy>());
	}


	public BaseEnemy Spawn(AssetReferenceGameObject prefabRef, Vector3 pos, Quaternion rot)
	{
		var enemy = pools[prefabRef].Spawn(pos, rot);
		if(enemy != null)
		{
			var mover = enemy.GetComponent<ObjectMover>();
			mover.OnOutofBounds += HandleOutofBounds;
		}
		return enemy;
	}

	public void Despawn(BaseEnemy instance)
		=> pools[instance.PrefabRef].Despawn(instance);

}