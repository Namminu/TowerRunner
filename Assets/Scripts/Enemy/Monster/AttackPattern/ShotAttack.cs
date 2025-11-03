using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static UnityEditor.Rendering.CameraUI;

public class ShotAttack : AttackPattern
{
	[SerializeField]
	private ProjectileType projectile;

	private Transform shotPoint;

	public void SetShotPoint(Transform point)
	{
		shotPoint = point;
	}

	protected override IEnumerator AttackRoutine(float damage, float delayTime)
	{
		yield return new WaitForSeconds(delayTime);

		// ProjectilePoolingManager의 id(enum) 기반 Spawn 사용
		if (ProjectilePoolingManager.Instance != null)
		{
			var proj = ProjectilePoolingManager.Instance.Spawn(projectile, shotPoint, damage);
			if (proj == null)
				Debug.LogWarning("[ShotAttack] Projectile spawn returned null (pool not ready?)");
		}
		else
		{
			// 폴백: 기존 Addressables 방식 (간단 로그)
			Debug.LogWarning("[ShotAttack] ProjectilePoolingManager not available; consider using Addressables.InstantiateAsync fallback if needed.");
		}
	}
}