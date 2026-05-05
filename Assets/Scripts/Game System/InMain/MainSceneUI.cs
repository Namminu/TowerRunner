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
	private Scenes nextSceneName;

	public void InitUI()
	{
		startBtn.onClick.RemoveAllListeners();
		exitBtn.onClick.RemoveAllListeners();

		startBtn.onClick.AddListener(() => LinkStartBtn());
		exitBtn.onClick.AddListener(() => LinkExitBtn());

		//Debug.Log("MainScene UI Inited");
	}

	private void LinkStartBtn()
	{
		AudioManager.Instance.PlaySound(AudioID.ButtonClick);

		ManagersInitializer.Instance.SceneLoad(nextSceneName);
	}

	private async void LinkExitBtn()
	{
		AudioManager.Instance.PlaySound(AudioID.ButtonClick);

		await SaveService.SaveAllAsync();
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}

	private void OnDisable()
	{
		startBtn.onClick.RemoveAllListeners();
		exitBtn.onClick.RemoveAllListeners();
	}
}
