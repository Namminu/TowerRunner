using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TowerStateTextUI : MonoBehaviour
{
	[SerializeField] private Text ReadyText;
	public Text ReadyStateText => ReadyText;
	[SerializeField] private Text RunText;
	[SerializeField] private float RunTextShowTime;

	private void Awake()
	{
		ReadyText.gameObject.SetActive(true);
		RunText.gameObject.SetActive(false);
	}

	private void OnEnable()
	{
		GameBus.Publish(new StateUIReady(this));
	}

	public void StartRunState() => StartCoroutine(RunRoutine());

	private IEnumerator RunRoutine()
	{
		ReadyText.gameObject.SetActive(false);
		ReadyText.gameObject.SetActive(true);
		yield return new WaitForSeconds(RunTextShowTime);
		ReadyText.gameObject.SetActive(false);
	}
}
