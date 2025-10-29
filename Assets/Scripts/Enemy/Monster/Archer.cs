using UnityEngine;

public class Archer : BaseMonster
{
	[SerializeField]
	private Transform shotPoint;

	public void OnArrowShotTrigged()
	{
		if(_attackPattern != null && _attackPattern is ShotAttack shotAtk)
		{
			shotAtk.SpawnProjectile(shotPoint, AttackDamage);
		}
	}
}
