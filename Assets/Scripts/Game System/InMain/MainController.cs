using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MainController : MonoBehaviour, IInitializable
{
	[Header("Canvas")]
	[SerializeField]
	private Button startBtn;
	[SerializeField]
	private Button exitBtn;

	[Header("Scene Controll")]
	[SerializeField]
	private SceneConfig sceneConfig;
	[SerializeField]
	private Scenes nextSceneName;

	private IEnumerator LinkStartBtn()
	{
		yield return sceneConfig.LoadSceneRoutine(nextSceneName);
	}

	private void LinkExitBtn() => Application.Quit();

	public void Init()
	{
		startBtn.onClick.RemoveAllListeners();
		exitBtn.onClick.RemoveAllListeners();

		startBtn.onClick.AddListener(() => StartCoroutine(LinkStartBtn()));
		exitBtn.onClick.AddListener(() => LinkExitBtn());
	}

	private void OnDisable()
	{
		startBtn.onClick.RemoveAllListeners();
		exitBtn.onClick.RemoveAllListeners();
	}
}
