using UnityEngine;
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

	public void InitUI()
	{
		/* About Remove Listener */
		volumeSlider.onValueChanged.RemoveAllListeners();
		displaySlider.onValueChanged.RemoveAllListeners();
		reviewBtn.onClick.RemoveAllListeners();
		resetBtn.onClick.RemoveAllListeners();
		exitBtn.onClick.RemoveAllListeners();

		/* About Volume */
		volumeSlider.value = AudioManager.Instance.MasterVolume;
		volumeSlider.onValueChanged.AddListener(OnVolumeSliderChanged);

		/* About Display */
		displaySlider.value = 0f;
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
			onYes: () => Application.OpenURL("")
			);
	}
	#endregion

	#region --- Reset ---
	private void OnDataResetBtnClicked()
	{
		popup.Show(
			"게임 데이터를 초기화하시겠습니까?",
			onYes: null);
	}
	#endregion

	#region --- Exit ---
	private void OnExitBtnClicked()
	{
		popup.Show(
			"게임을 종료합니다",
			onYes: () =>
			{
#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
			});
	}
	#endregion

	private void OnDisable()
	{


		SaveVolumeSetting();
		volumeSlider.onValueChanged.RemoveListener(OnVolumeSliderChanged);
		displaySlider.onValueChanged.RemoveListener(OnDisPlaySliderChanged);
		reviewBtn.onClick.RemoveListener(OnReviewBtnClicked);
		resetBtn.onClick.RemoveListener(OnDataResetBtnClicked);
		exitBtn.onClick.RemoveListener(OnExitBtnClicked);
	}
}
