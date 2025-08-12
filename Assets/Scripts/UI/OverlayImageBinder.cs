using UnityEngine;
using UnityEngine.UI;

public class OverlayImageBinder : MonoBehaviour
{
	[SerializeField] private Image overlay;

	private void OnEnable()
	{
		BrightnessManager.Instance?.RegisterOverlay(overlay);
	}

	private void OnDisable()
	{
		BrightnessManager.Instance?.UnregisterOverlay(overlay);
	}
}
