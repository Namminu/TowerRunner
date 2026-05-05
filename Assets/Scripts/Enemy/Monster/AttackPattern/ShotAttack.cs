using System.Collections;
using UnityEngine;

public class ShotAttack : AttackPattern
{
	[SerializeField]
	private ProjectileType projectile;

	private Transform[] shotPoint;

	public void SetShotPoint(Transform[] point)
	{
		shotPoint = new Transform[point.Length];
		for (int i= 0; i < point.Length; i++)
		{
			shotPoint[i] = point[i];
		}
	}

	protected override IEnumerator AttackRoutine(float damage, float delayTime)
	{
		yield return new WaitForSeconds(delayTime);

		if (ProjectilePoolingManager.Instance == null)
		{
			Debug.LogWarning("[ShotAttack] ProjectilePoolingManager Instance is null.");
			yield break;
		}

		var proj = ProjectilePoolingManager.Instance.Spawn(projectile, shotPoint, damage);
		if(proj == null)
		{
			Debug.LogWarning("[ShotAttack] Spawn Method return nothing");
		}
	}
}