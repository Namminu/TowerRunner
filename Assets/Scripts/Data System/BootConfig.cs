using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "BootConfig", menuName = "Scriptable Objects/BootConfig")]
public class BootConfig : ScriptableObject
{
    [Header("Addressables")]
	public AssetReferenceT<EnemyData> enemyDataRef;
	public AssetReferenceT<ItemDatabase> itemDBRef;
	public AssetReferenceT<EnforceDatabase> enforceDBRef;

#if UNITY_EDITOR
	private void OnValidate()
	{
		if (enemyDataRef == null || !enemyDataRef.RuntimeKeyIsValid())
			Debug.LogError("[Boot Config] enemyDataRef Miss");
		if (itemDBRef == null || !itemDBRef.RuntimeKeyIsValid())
			Debug.LogError("[Boot Config] itemDBRef Miss");
		if (enforceDBRef == null || !enforceDBRef.RuntimeKeyIsValid())
			Debug.LogError("[Boot Config] enforceDBRef Miss");
	}
#endif
}
