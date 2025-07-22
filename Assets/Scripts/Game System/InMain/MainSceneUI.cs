using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneUI : MonoBehaviour, ISceneUI
{
	[Header("UI")]
	[SerializeField]
	private Button startBtn;
	[SerializeField] 
	private Button exitBtn;

	[Header("Scene Controll")]
	[SerializeField]
	private SceneConfig sceneConfig;
	[SerializeField]
	private Scenes nextSceneName;

	public void InitUI()
	{
		startBtn.onClick.RemoveAllListeners();
		exitBtn.onClick.RemoveAllListeners();

		startBtn.onClick.AddListener(() => StartCoroutine(LinkStartBtn()));
		exitBtn.onClick.AddListener(() => LinkExitBtn());
	}

	private IEnumerator LinkStartBtn()
	{
		Debug.Log("MainScene Start Btn Clicked");
		yield return ManagersInitializer.Instance.InitializeSceneManagers(nextSceneName);
		yield return SceneUIManager.Instance.LoadUIForScene(nextSceneName);
		yield return sceneConfig.LoadSceneRoutine(nextSceneName);
	}

	private void LinkExitBtn()
	{
		Debug.Log("MainScene Exit Btn Clicked");
		Application.Quit();
	}

	private void OnDisable()
	{
		startBtn.onClick.RemoveAllListeners();
		exitBtn.onClick.RemoveAllListeners();
	}
}
