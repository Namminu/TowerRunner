using UnityEngine;
using UnityEngine.UI;

public class TimerUIEntry : MonoBehaviour, IPoolable
{
	[SerializeField]
	private Image timerIcon;
	[SerializeField]
	private Image fillImage;

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
		fillImage.fillAmount = 0f;
		gameObject.SetActive(true);
	}

	private void Update()
	{
		remain -= Time.deltaTime;
		if(remain <= 0f)
		{
			fillImage.fillAmount = 1f;
			onFinish?.Invoke();
			return;
		}
		fillImage.fillAmount = 1 - (remain / totalDuration);
	}
}
