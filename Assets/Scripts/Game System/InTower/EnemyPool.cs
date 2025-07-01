using UnityEngine;

public class EnemyPool : ObjectPool<BaseEnemy>
{
	public EnemyPool(BaseEnemy prefab, int size, Transform parent = null)
		: base(prefab, size, parent) { }

	protected override BaseEnemy CreateNew()
	{
		var inst = base.CreateNew();
		inst.Prefab = prefab;
		return inst;
	}
}