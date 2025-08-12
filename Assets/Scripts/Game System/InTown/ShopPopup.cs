using Unity.Android.Gradle;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopPopup : MonoBehaviour
{
	[Header("Rooot Popup Panel")]
	[SerializeField] private GameObject popupRoot;

	[Header("Basic Popup UI")]
	[SerializeField] private GameObject BasicUI;
	[SerializeField] private Image itemIcon;
	[SerializeField] private Text itemName;
	[SerializeField] private Text itemDescription;
	[SerializeField] private Button yesBtn;
	[SerializeField] private Button noBtn;

	[Header("Warning Popup UI")]
	[SerializeField] private GameObject WarningUI;
	[SerializeField] private Text warningText;
	[SerializeField] private Button closeBtn;

	private ItemData currentItem;

	private void Awake()
	{
		gameObject.SetActive(false);
	}

	private void OnEnable()
	{
		BasicUI.SetActive(true);
		WarningUI.SetActive(false);

		GameEvents.OnShopItemSeledted += ShowPopup;
		GameEvents.OnInvenFull += OnInvenFull;
		GameEvents.OnShortageGold += OnShortageGold;
	}

	private void OnDisable()
	{
		BasicUI.SetActive(true);
		WarningUI.SetActive(false);

		GameEvents.OnShopItemSeledted -= ShowPopup;
		GameEvents.OnInvenFull -= OnInvenFull;
		GameEvents.OnShortageGold -= OnShortageGold;
	}

	private void ShowPopup(ItemData item)
	{
		currentItem = item;

		itemIcon.sprite = item.itemIcon;
		itemName.text = item.name;
		itemDescription.text = item.itemDescription;

		popupRoot.SetActive(true);
		yesBtn.onClick.RemoveAllListeners();
		yesBtn.onClick.AddListener(OnConfirm);
	}

	private void OnConfirm()
	{
		GameEvents.RaiseShopItemBuyConfirmed(currentItem);
		popupRoot.SetActive(false);
	}

	private void OnInvenFull()
	{
		BasicUI.SetActive(false);
		warningText.text = "인벤토리에 남은 공간이 없습니다";
		WarningUI.SetActive(true);
	}

	private void OnShortageGold()
	{
		BasicUI.SetActive(false);
		warningText.text = "보유 골드가 충분하지 않습니다";
		WarningUI.SetActive(true);
	}
}
