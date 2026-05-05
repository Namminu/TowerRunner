using UnityEngine;
using UnityEngine.UI;

public class ShopItemSlot : MonoBehaviour
{
	[SerializeField] private Button itemSlotBtn;
	[SerializeField] private Image itemIcon;
	[SerializeField] private Text itemName;
	[SerializeField] private Text itemPrice;
	[SerializeField] private Image itemCover;

	private ItemData data;
	public ItemData Data => data;


	private void OnEnable()
	{
		itemSlotBtn.onClick.AddListener(OnClicked);
	}

	private void OnClicked()
	{
		AudioManager.Instance.PlaySound(AudioID.ButtonClick);
		GameEvents.RaiseShopItemSeleted(data);
	}

	private void OnDisable()
	{
		itemSlotBtn.onClick.RemoveAllListeners();
	}

	public void Setup(ItemData item)
	{
		data = item;
		itemIcon.sprite = item.itemIcon;
		itemName.text = item.itemName;
		itemPrice.text = item.itemPrice.ToString();
		itemCover.gameObject.SetActive(false);
	}

	public void MarkPurchased()
	{
		itemCover.gameObject.SetActive(true);
		GetComponent<Button>().interactable = false;
	}

}
