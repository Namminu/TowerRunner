using UnityEngine;
using UnityEngine.UI;

public interface ISceneUI
{
	/// <summary>
	/// This Function Called After Change Scene & UI Prefab Instantiate
	/// </summary>
	void InitUI();
}

public class BootSceneUI : MonoBehaviour, ISceneUI
{
	[Header("UI")]
	[SerializeField] private Slider progressBar;
	[SerializeField] private Text initStateText;
	[SerializeField] private Button retryBtn;

	public void InitUI()
	{
		retryBtn.onClick.RemoveAllListeners();
		retryBtn.onClick.AddListener(() =>
		{
			retryBtn.gameObject.SetActive(false);
			BootSceneController.Instance.RestartBootRoutine();
		});
		retryBtn.gameObject.SetActive(false);
	}

	public void UpdateProgress(float percent) => progressBar.value = percent;
	public void UpdateText(string text) => initStateText.text = text;

	public void ShowRetryBtn() => retryBtn.gameObject.SetActive(true);
}
