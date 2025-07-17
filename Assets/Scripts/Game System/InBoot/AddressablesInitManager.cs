using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class AddressablesInitManager : MonoBehaviour
{
	[Header("Addressables Refs")]
	[SerializeField, Tooltip("EnemyData SO Address")]
	private AssetReferenceT<EnemyData> enemyDataRef;
	[SerializeField, Tooltip("ItemDatabase SO Address")]
	private AssetReferenceT<ItemDatabase> itemDBRef;

	[SerializeField]
	private Scenes nextSceneName;

	[Header("UI")]
	[SerializeField]
	private Slider progressBar;
	[SerializeField]
	private Text initStateText;

	[Header("Retry")]
	[SerializeField]
	private Button retryBtn;

	public static event Action<EnemyData, ItemDatabase> OnInitialized;

	private IEnumerator Start()
	{
		yield return Addressables.InitializeAsync();

		var enemyDataOp = enemyDataRef.LoadAssetAsync();
		var itemDBOp = itemDBRef.LoadAssetAsync();
		yield return enemyDataOp;
		yield return itemDBOp;

		OnInitialized?.Invoke(enemyDataOp.Result, itemDBOp.Result);

		Addressables.LoadSceneAsync(nextSceneName);
	}
}
