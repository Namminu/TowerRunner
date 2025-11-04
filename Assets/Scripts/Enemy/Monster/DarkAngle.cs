using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DarkAngle : BaseMonster
{
	[SerializeField]
	private AssetReferenceGameObject sicklePrefs;

	private AsyncOperationHandle<GameObject> prefabHandle;
	private GameObject sickleInstance;

	protected override void Awake()
	{
		base.Awake();
		prefabHandle = sicklePrefs.LoadAssetAsync<GameObject>();
	}

	public void OnSickleAnimationTrigged()
	{
		if (sickleInstance != null)
		{
			sickleInstance.SetActive(true);
		}
		else
		{
			if (prefabHandle.Status == AsyncOperationStatus.Succeeded)
			{
				sickleInstance = GameObject.Instantiate(prefabHandle.Result, transform);
				sickleInstance.SetActive(true);
			}
		}
	}

	public override void OnAttackAnimationEnd()
	{
		base.OnAttackAnimationEnd();
		if (sickleInstance != null)
		{
			sickleInstance.SetActive(false);
		}
	}

	private void OnDestroy()
	{
		if (sickleInstance != null)
		{
			Destroy(sickleInstance);
			sickleInstance = null;
		}

		if (prefabHandle.IsValid())
		{
			Addressables.Release(prefabHandle);
		}
	}
}
