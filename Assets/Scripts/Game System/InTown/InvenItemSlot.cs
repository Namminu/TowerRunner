using UnityEngine;
using UnityEngine.UI;

public class InvenItemSlot : MonoBehaviour
{
	[SerializeField] private Image itemIcon;
	[SerializeField] private Button slotButton;

	public Button Button => slotButton;

	public void SetData(ItemData item)
	{
		if (item == null)
		{
			itemIcon.sprite = null;
			itemIcon.gameObject.SetActive(false);
			return;
		}

		itemIcon.sprite = item.itemIcon;
		itemIcon.gameObject.SetActive(true);
		slotButton.interactable = true;
	}

	public void SetEmpty()
	{
		itemIcon.gameObject.SetActive(false);
		slotButton.interactable = false;
	}
}
