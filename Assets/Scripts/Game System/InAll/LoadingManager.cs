using System;
using System.Collections;
using UnityEngine;

public class LoadingManager : MonoBehaviour, IInitializable
{
	public void Init()
	{
		StartCoroutine(LoadRoutine());
	}

	private IEnumerator LoadRoutine()
	{
		yield return new WaitForSecondsRealtime(2.5f);

		ManagersInitializer.Instance.SceneLoad(Scenes.Tower);
	}
}
