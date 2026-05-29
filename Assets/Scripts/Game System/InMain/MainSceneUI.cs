using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Analytics;

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


		//FirebaseManager.LogEvent(
		//	"test_event_mainscene",
		//	new Parameter("test_value", 1)
		//);

		//Debug.Log("Test Event Send");
	}

	private void LinkStartBtn()
	{
		AudioManager.Instance.PlaySound(AudioID.ButtonClick);

		ManagersInitializer.Instance.SceneLoad(nextSceneName);
		FirebaseManager.LogEvent("game_start");
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
		FirebaseManager.LogEvent("game_end");
	}

	private void OnDisable()
	{
		startBtn.onClick.RemoveAllListeners();
		exitBtn.onClick.RemoveAllListeners();
	}
}
