using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ShotAttack : AttackPattern
{
	[SerializeField]
	private AssetReferenceGameObject projectile;

	//private AsyncOperationHandle<GameObject> prefabHandle;
	//private bool _prefabLoaded = false;

	private void Awake()
	{
		//prefabHandle = projectile.LoadAssetAsync<GameObject>();
	}

	public override void ExecuteAttack(float dmg)
	{
		/* 발사체 생성 타이밍은 애니메이션 이벤트로 호출되어서
		 여기서는 크게 진행할 동작이 없음 */
	}

	public void SpawnProjectile(Transform shotPoint, float damage)
	{
		/*
		 1. Addressables Projectile 생성
		 2. 생성하고 IProjectile 인터페이스로 접근해서 InitializeProjectile(shotPoint, damage)
			>> 월드 좌표로 생성되는지, 생성 후 스케일은 정상적으로 적용되었는지, 데미지 적용됐는지 체크
		 3. 그리고 Destroy 되고 나서 데이터 Release 되었는지 체크
		    >> 이건 어떻게 확인하지?
		 */
	}
}

/*
근데 일단 이 방향성이 맞는지 잘 모르겠음.
IProjectile 인터페이스 두는건 맞아 => 발사체가 여러개일거니까
근데 ExecuteAttack 을 만들어놓고 원거리 공격이라고 안쓰면서, SpawnProjectile 을 또 만드는거는
같은 의도의 코드를 중복해서 만드는 듯한 느낌을 받음
executeAttack 을 제대로 활용하면서, OnArrowShotTrigged 로 발사 타이밍을 조절하는 방법은 없을까?

지금 떠오르는건 ExecuteAttack 에서 Projectile 생성 -> 생성하면서 데미지 세팅하고
ShotPoint 위치에 생성하든, 자식 오브젝트로 붙이든 발사체 세팅만 해두는거
그러고 OnArrowShotTrigged 로 물리적인 힘이든, 강제 이동이든 이때 발사시키는거지
Destroy/Release 조건은 기존과 동일하게
 */