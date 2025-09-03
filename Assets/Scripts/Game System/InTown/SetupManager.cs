using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SetupManager : MonoBehaviour, ISceneUI
{
	[Header("Volume Zone")]
	[SerializeField] private Slider volumeSlider;

	[Header("Display Zone")]
	[SerializeField] private Slider displaySlider;

	[Header("Review Zone")]
	[SerializeField] private Button reviewBtn;

	[Header("Reset Zone")]
	[SerializeField] private Button resetBtn;
	[SerializeField] private SceneConfig sceneConfig;

	[Header("Exit Zone")]
	[SerializeField] private Button exitBtn;

	[Header("Pop Up UI")]
	[SerializeField] private SettingPopup popup;

	public void InitUI()
	{
		/* About Remove Listener */
		volumeSlider.onValueChanged.RemoveAllListeners();
		displaySlider.onValueChanged.RemoveAllListeners();
		reviewBtn.onClick.RemoveAllListeners();
		resetBtn.onClick.RemoveAllListeners();
		exitBtn.onClick.RemoveAllListeners();

		/* About Volume */
		volumeSlider.value = Prefs.MasterVolume;
		volumeSlider.onValueChanged.AddListener(OnVolumeSliderChanged);

		/* About Display */
		displaySlider.value = Prefs.DisplayBrightness;
		displaySlider.onValueChanged.AddListener(OnDisPlaySliderChanged);

		/* About Review */
		reviewBtn.onClick.AddListener(OnReviewBtnClicked);

		/* About Reset */
		resetBtn.onClick.AddListener(OnDataResetBtnClicked);

		/* About Exit */
		exitBtn.onClick.AddListener(OnExitBtnClicked);
	}

	#region --- Volume ---
	private void OnVolumeSliderChanged(float value)
	{
		AudioManager.Instance.SetMasterVolume(value);
	}

	private void SaveVolumeSetting()
	{
		float value = volumeSlider.value;

	}
	#endregion

	#region --- Display ---
	private void OnDisPlaySliderChanged(float value)
	{
		BrightnessManager.Instance.SetBrightness(value);
	}
	#endregion

	#region --- Review ---
	private void OnReviewBtnClicked()
	{
		popup.Show(
			"리뷰를 작성하러 이동합니다",
			onYes: () => Application.OpenURL("https://play.google.com/store/games?hl=ko")
			);
	}
	#endregion

	#region --- Reset ---
	private void OnDataResetBtnClicked()
	{
		popup.Show(
			"게임 데이터를 초기화하시겠습니까?",
			onYes: () => StartCoroutine(ResetRoutine())
			);
	}

	private IEnumerator ResetRoutine()
	{
		SaveService.ResetAll();

		UIManager.Instance.UnLoadCurrentUI();
		AddressablesTracker.ReleaseAll();

		yield return null;

		yield return ManagersInitializer.Instance.InitializeSceneManagers(Scenes.BootScene);
		yield return sceneConfig.LoadSceneRoutine(Scenes.BootScene);
	}
	#endregion

	#region --- Exit ---
	private void OnExitBtnClicked()
	{
		popup.Show(
			"게임을 종료합니다",
			onYes: async () =>
			{
				await SaveService.SaveAllAsync();
#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
			});
	}
	#endregion

	private void OnDestroy()
	{
		popup.gameObject.SetActive(false);

		volumeSlider.onValueChanged.RemoveListener(OnVolumeSliderChanged);
		displaySlider.onValueChanged.RemoveListener(OnDisPlaySliderChanged);
		reviewBtn.onClick.RemoveListener(OnReviewBtnClicked);
		resetBtn.onClick.RemoveListener(OnDataResetBtnClicked);
		exitBtn.onClick.RemoveListener(OnExitBtnClicked);
	}
}
