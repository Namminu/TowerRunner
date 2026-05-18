using System.Collections;
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
	[SerializeField] private Button RetryButton;
	[SerializeField] private Button ExitButton;

	[Header("Game Over")]
	[SerializeField] private Image GameOverPanel;
	//[SerializeField] private Button GameSessionCloseButton;
	[SerializeField] private EndGamePanelUI EndGamePanelUI;

	[Header("Pause Count")]
	[SerializeField] private Text countdownText;

	private void Awake()
	{
		if(EndGamePanelUI == null)
		{
			EndGamePanelUI = GetComponentInChildren<EndGamePanelUI>();
		}
	}

	public void InitUI()
	{
		GameEvents.OnBattleEnded += HandlePlayerDeathGameEnd;

		/* Setting Panel */
		SettingCloseButton.onClick.RemoveAllListeners();
		SettingCloseButton.onClick.AddListener(() => CloseSettingPanel());

		/* Setting Panel : Volume */
		volumeSlider.value = Prefs.MasterVolume;
		volumeSlider.onValueChanged.AddListener(OnVolumeSliderChanged);

		/* Setting Panel : Brightness */
		brightnessSlider.value = Prefs.DisplayBrightness;
		brightnessSlider.onValueChanged.AddListener(OnDisPlaySliderChanged);

		/* Setting Panel : Retry */
		RetryButton.onClick.RemoveAllListeners();
		RetryButton.onClick.AddListener(() => RetryGameOnStart());

		/* Setting Panel : Exit */
		ExitButton.onClick.RemoveAllListeners();
		ExitButton.onClick.AddListener(() => ExitGameAndLoadTownScene());
	}

	public void ShowPanel(TowerPanelType OnType)
	{
		PanelBG.gameObject.SetActive(true);
		switch (OnType)
		{
			case TowerPanelType.Setting:
				GameOverPanel.gameObject.SetActive(false);
				SettingPanel.gameObject.SetActive(true);

				SetSavedSettingValue();
				break;
			case TowerPanelType.GameOver:
				SettingPanel.gameObject.SetActive(false);
				GameOverPanel.gameObject.SetActive(true);
				break;
		}
	}

	#region --- Setting Panel ---
	
	private void SetSavedSettingValue()
	{
		volumeSlider.value = AudioManager.Instance.MasterVolume;
		brightnessSlider.value = BrightnessManager.Instance.CurrentBrightness;
	}

	private void OnVolumeSliderChanged(float value)
	{
		AudioManager.Instance.SetMasterVolume(value);
	}

	private void OnDisPlaySliderChanged(float value)
	{
		BrightnessManager.Instance.SetBrightness(value);
	}

	private void RetryGameOnStart()
	{
		AudioManager.Instance.PlaySound(AudioID.ButtonClick);
		GameEvents.RaiseBattleEnd();
		RetryGameRoutiune();
		FirebaseManager.LogEvent("Restart Game in Tower");
	}

	private void RetryGameRoutiune()
	{
		ManagersInitializer.Instance.SceneLoad(Scenes.Loading);
	}

	private void ExitGameAndLoadTownScene()
	{
		AudioManager.Instance.PlaySound(AudioID.ButtonClick);

		PanelBG.gameObject.SetActive(false);
		SettingPanel.gameObject.SetActive(false);

		// 무적 해제 후 즉사 처리
		Player.Instance.SetInvincible(false);
		Player.Instance.ImmediateDeath();

		FirebaseManager.LogEvent("Exit Game In Tower");
	}

	public void CloseSettingPanel()
	{
		AudioManager.Instance.PlaySound(AudioID.ButtonClick);

		SettingPanel.gameObject.SetActive(false);
		PanelBG.gameObject.SetActive(false);

		StartCoroutine(EnsumeGameCount());
	}

	private IEnumerator EnsumeGameCount()
	{
		countdownText.gameObject.SetActive(true);

		int count = 3;
		WaitForSecondsRealtime waitTime = new(1);
		while(count > 0)
		{
			countdownText.text = count.ToString();
			yield return waitTime;

			count--;
		}

		countdownText.gameObject.SetActive(false);
		// 게임 재개
		GameStateManager.Instance.SetState(GameStateManager.GameState.Play);
		AudioManager.Instance.ResumeBGM();
	}
	#endregion


	#region --- Game Over Panel ---

	private void HandlePlayerDeathGameEnd()
	{
		StartCoroutine(GameEndRoutine());
	}

	private IEnumerator GameEndRoutine()
	{
		GameStateManager.Instance.SetState(GameStateManager.GameState.GameOver);

		yield return new WaitForSecondsRealtime(1.5f);

		PanelBG.gameObject.SetActive(true);
		GameOverPanel.gameObject.SetActive(true);

		if (EndGamePanelUI)
		{
			EndGamePanelUI.HandleGameEndCheck();
		}
	}

	#endregion

	private void OnDisable()
	{
		GameEvents.OnBattleEnded -= HandlePlayerDeathGameEnd;

		SettingCloseButton.onClick.RemoveAllListeners();
		RetryButton.onClick.RemoveAllListeners();
		ExitButton.onClick.RemoveAllListeners();
	}
}
