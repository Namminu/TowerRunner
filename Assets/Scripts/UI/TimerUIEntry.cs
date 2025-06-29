using UnityEngine;
using UnityEngine.UI;

public class TimerUIEntry : MonoBehaviour, IPoolable
{
	[SerializeField]
	private Image timerIcon;
	[SerializeField]
	private Image fillImage;
	[SerializeField]
	private Text timerText;

	private float remain;
	private float totalDuration;
	System.Action onFinish;

	public void OnDespawn()
	{
		gameObject.SetActive(false);
	}

	public void OnSpawn()
	{
		gameObject.SetActive(true);
	}

	public void Setup(Sprite icon, float duration, System.Action onFinish)
	{
		this.onFinish = onFinish;
		totalDuration = duration;
		remain = duration;
		timerIcon.sprite = icon;
		fillImage.fillAmount = 1f;
		timerText.text = duration.ToString("F0");
		gameObject.SetActive(true);
	}

	private void Update()
	{
		remain -= Time.deltaTime;
		if(remain <= 0f)
		{
			onFinish?.Invoke();
			return;
		}
		fillImage.fillAmount = remain / totalDuration;
		timerText.text = remain.ToString("F0");
	}
}
