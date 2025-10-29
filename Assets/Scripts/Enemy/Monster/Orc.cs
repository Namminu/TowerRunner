using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Orc : BaseMonster
{
	[SerializeField]
	private AssetReferenceGameObject thunderPrefs;

	private AsyncOperationHandle<GameObject> prefabHandle;
	private GameObject thunderInstance;

	protected override void Awake()
	{
		base.Awake();
		prefabHandle = thunderPrefs.LoadAssetAsync<GameObject>();
	}

	public void OnTunderAnimationTrigged()
	{
		if(thunderInstance != null)
		{
			thunderInstance.SetActive(true);
		}
		else
		{
			if(prefabHandle.Status == AsyncOperationStatus.Succeeded)
			{
				thunderInstance = GameObject.Instantiate(prefabHandle.Result, transform);
				thunderInstance.SetActive(true);
			}
		}
	}

	public override void OnAttackAnimationEnd()
	{
		base.OnAttackAnimationEnd();
		if(thunderInstance != null)
		{
			thunderInstance.SetActive(false);
		}
	}

	private void OnDestroy()
	{
		if(thunderInstance != null )
		{
			Destroy(thunderInstance);
			thunderInstance = null;
		}

		if(prefabHandle.IsValid())
		{
			Addressables.Release(prefabHandle);
		}
	}
}
