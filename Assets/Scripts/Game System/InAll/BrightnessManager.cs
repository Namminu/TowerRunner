using UnityEngine;
using UnityEngine.UI;

public class BrightnessManager : MonoBehaviour, IInitializable
{
	public static BrightnessManager Instance { get; private set; }

	[SerializeField] private Image overlayImage;
	private const byte MaxAlphaByte = 240;

	private float _currentBrightness = 0.5f;
	public float CurrentBrightness => _currentBrightness;

	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	private void Start()
	{
		overlayImage = UIManager.Instance.OverlayImage;
		if (overlayImage == null)
		{
			Debug.LogError("Cannot Find Overlay Image");
			FirebaseManager.LogCrash("Cannot Find Overlay Image");
		}
	}

	private void ApplyDisplayBrightness(float value)
	{
		if (overlayImage == null)
		{
			Debug.Log("Overlay Image Null Reference Error");
			FirebaseManager.LogCrash("Cannot Find Overlay Image");
			return;
		}

		_currentBrightness = Mathf.Clamp01(value);
		Prefs.DisplayBrightness = _currentBrightness;

		float alpha = (1f - Mathf.Clamp01(value)) * (MaxAlphaByte / 255f);
		Color c = overlayImage.color;
		c.a = alpha;
		overlayImage.color = c;
	}

	public void RegisterOverlay(Image image)
	{
		overlayImage = image;
		if(overlayImage != null)
		{
			overlayImage.raycastTarget = false;
			ApplyDisplayBrightness(Prefs.DisplayBrightness);
		}
	}

	public void UnregisterOverlay(Image image)
	{
		if (overlayImage == image)
			overlayImage = null;
	}

	public void Init()
	{
		ApplyDisplayBrightness(Prefs.DisplayBrightness);
	}

	public void SetBrightness(float value)
	{
		ApplyDisplayBrightness(value);
	}
}
