using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class MainController : MonoBehaviour, IInitializable
{
	[SerializeField] private AssetReferenceGameObject playerRef;

	public void Init()
	{
		AudioManager.Instance.PlayBGM(AudioID.MainBGM);

		StartCoroutine(CreatePlayer());

		return;
	}

	private IEnumerator CreatePlayer()
	{
		if(Player.Instance != null)
		{
			yield break;
		}

		var handle = playerRef.InstantiateAsync();
		yield return handle;

		if (handle.Status != AsyncOperationStatus.Succeeded)
		{
			Debug.LogError($"{playerRef.RuntimeKey} Load Failed");
			yield break;
		}

		AddressablesTracker.Track(handle, isPersistent: false);

		var root = handle.Result;
		DontDestroyOnLoad(root);

		root.SetActive(false);
	}
}
