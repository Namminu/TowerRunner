using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingPopup : MonoBehaviour, ISceneUI
{
	[SerializeField] private Text titleMessage;
	[SerializeField] private Button yesBtn;
	[SerializeField] private Button noBtn;

	[SerializeField] private CanvasGroup canvasGroup; 

	private Action _onYes;
	private Action _onNo;


	public void InitUI()
	{
		if(canvasGroup == null)
		{
			canvasGroup = GetComponent<CanvasGroup>();
		}

		Hide();

		yesBtn.onClick.AddListener(OnYes);
		noBtn.onClick.AddListener(OnNo);
	}

	public void Show(string message, Action onYes, Action onNo = null)
	{
		canvasGroup.alpha = 1;
		canvasGroup.interactable = true;
		canvasGroup.blocksRaycasts = true;

		titleMessage.text = message;
		_onYes = onYes;
		_onNo = onNo;

		Canvas.ForceUpdateCanvases();
	}

	private void OnYes()
	{
		_onYes?.Invoke();
		Clear();
		Hide();
	}

	private void OnNo() 
	{
		_onNo?.Invoke();
		Clear();
		Hide();
	}

	private void Hide()
	{
		canvasGroup.alpha = 0;
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
	}

	private void Clear()
	{
		_onYes = null;
		_onNo = null;
	}

	private void OnDisable()
	{
		Clear();
		Hide();
		yesBtn.onClick.RemoveAllListeners();
		noBtn.onClick.RemoveAllListeners();
	}
}
