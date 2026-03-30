using UnityEngine;
using UnityEngine.UI;

public enum TowerPanelType
{
	Setting,
	GameOver
}

public class TowerPanelUI : MonoBehaviour
{
	[Header("BG")]
	[SerializeField] private Image PanelBG;

	[Header("Setting")]
	[SerializeField] private Image SettingPanel;
	[SerializeField] private Button SettingCloseButton;

	[Header("Setting|Content")]
	[SerializeField] private Slider volumeSlider;
	[SerializeField] private Slider brightnessSlider;
	[SerializeField] private Button vibrateButton;
	[SerializeField] private Button RetryButton;
	[SerializeField] private Button ExitButton;

	[Header("Game Over")]
	[SerializeField] private Image GameOverPanel;
	[SerializeField] private Button GameSessionCloseButton;

	private void Awake()
	{
		/* Setting Panel */ 
		SettingCloseButton.onClick.RemoveAllListeners();
		SettingCloseButton.onClick.AddListener(() => CloseSettingPanel());

		/* Setting Panel : Volume */
		volumeSlider.value = Prefs.MasterVolume;
		volumeSlider.onValueChanged.AddListener(OnVolumeSliderChanged);

		/* Setting Panel : Brightness */
		brightnessSlider.value = Prefs.DisplayBrightness;
		brightnessSlider.onValueChanged.AddListener(OnDisPlaySliderChanged);

		/* Setting Panel : Vibrate */
		vibrateButton.onClick.RemoveAllListeners();
		vibrateButton.onClick.AddListener(() => OnToggleVibrateSystem());

		/* Setting Panel : Retry */
		RetryButton.onClick.RemoveAllListeners();
		RetryButton.onClick.AddListener(() => RetryGameOnStart());

		/* Setting Panel : Exit */
		RetryButton.onClick.RemoveAllListeners();
		RetryButton.onClick.AddListener(() => ExitGameAndLoadTownScene());

		/* Game Over Panel */
		GameSessionCloseButton.onClick.RemoveAllListeners();
		GameSessionCloseButton.onClick.AddListener(() => CloseGameOverPanel());
	}

	public void ShowPanel(TowerPanelType OnType)
	{
		PanelBG.gameObject.SetActive(true);
		switch (OnType)
		{
			case TowerPanelType.Setting:
				GameOverPanel.gameObject.SetActive(false);
				SettingPanel.gameObject.SetActive(true);
				break;
			case TowerPanelType.GameOver:
				SettingPanel.gameObject.SetActive(false);
				GameOverPanel.gameObject.SetActive(true);
				break;
		}

		// 게임 멈추는 로직 필요

	}

	#region --- Setting Panel ---
	private void OnVolumeSliderChanged(float value)
	{
		AudioManager.Instance.SetMasterVolume(value);
	}

	private void SaveVolumeSetting()
	{
		float value = volumeSlider.value;

	}

	private void OnDisPlaySliderChanged(float value)
	{
		BrightnessManager.Instance.SetBrightness(value);
	}

	private void OnToggleVibrateSystem()
	{

	}

	private void RetryGameOnStart()
	{

	}

	private void ExitGameAndLoadTownScene()
	{

	}

	public void CloseSettingPanel()
	{
		SettingPanel.gameObject.SetActive(false);
		PanelBG.gameObject.SetActive(false);

		// 게임 재개하는 로직 필요

	}
	#endregion



	#region --- Game Over Panel ---


	public void CloseGameOverPanel()
	{
		PanelBG.gameObject.SetActive(false);
		GameOverPanel.gameObject.SetActive(false);

		// Town Scene 이동

	}
	#endregion
}
