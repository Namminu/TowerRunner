using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingPopup : MonoBehaviour
{
	[SerializeField] private GameObject settingPopup;
	[SerializeField] private Text titleMessage;
	[SerializeField] private Button yesBtn;
	[SerializeField] private Button noBtn;

	private Action _onYes;
	private Action _onNo;

	private void Awake()
	{
		settingPopup.SetActive(false);
		yesBtn.onClick.AddListener(OnYes);
		noBtn.onClick.AddListener(OnNo);
	}

	public void Show(string message, Action onYes, Action onNo = null)
	{
		titleMessage.text = message;
		_onYes = onYes;
		_onNo = onNo;
		settingPopup.SetActive(true);
	}

	private void OnYes()
	{
		settingPopup.SetActive(false);
		_onYes?.Invoke();
		Clear();
	}

	private void OnNo() 
	{
		settingPopup.SetActive(false);
		_onNo?.Invoke();
		Clear();
	}

	private void Clear()
	{
		_onYes = null;
		_onNo = null;
	}
}
