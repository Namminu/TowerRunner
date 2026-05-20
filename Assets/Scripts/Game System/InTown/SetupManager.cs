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

	[Header("Exit Zone")]
	[SerializeField] private Button exitBtn;

	[Header("Pop Up UI")]
	[SerializeField] private SettingPopup popup;

	/*private readonly string GamePlayURL = "https://play.google.com/store/games?hl=ko";*/
	private readonly string GamePlayURL = "https://play.google.com/store/apps/details?id=com.beistudio.towerrunner";

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
		AudioManager.Instance.PlaySound(AudioID.ButtonClick);

		popup.Show(
			"리뷰를 작성하러 이동합니다",
			onYes: () =>
			{
				Application.OpenURL(GamePlayURL);
				FirebaseManager.LogEvent("review");
			}
			);
	}
	#endregion

	#region --- Reset ---
	private void OnDataResetBtnClicked()
	{
		AudioManager.Instance.PlaySound(AudioID.ButtonClick);

		popup.Show(
			"게임 데이터를 초기화하시겠습니까?",
			onYes: () => ResetCall()
			);
	}

	private void ResetCall()
	{
		ManagersInitializer.Instance.ResetCall();
		FirebaseManager.LogEvent("reset_data");
	}
	#endregion

	#region --- Exit ---
	private void OnExitBtnClicked()
	{
		AudioManager.Instance.PlaySound(AudioID.ButtonClick);

		popup.Show(
			"게임을 종료합니다",
			onYes: async () =>
			{
				await SaveService.SaveAllAsync();
				AddressablesTracker.ReleaseAll();
#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
				FirebaseManager.LogEvent("game_end");
			});
	}
	#endregion

	private void OnEnable()
	{
		SetSavedSettingValue();
	}

	private void SetSavedSettingValue()
	{
		volumeSlider.value = AudioManager.Instance.MasterVolume;
		displaySlider.value = BrightnessManager.Instance.CurrentBrightness;
	}

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
