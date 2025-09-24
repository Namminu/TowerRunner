using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TowerStateTextUI : MonoBehaviour
{
	[SerializeField] private Text ReadyText;
	[SerializeField] private Text RunText;
	[SerializeField] private float RunTextShowTime;

	private void Awake()
	{
		ReadyText.gameObject.SetActive(true);
		RunText.gameObject.SetActive(false);
	}

	public void StartReadyState() => StartCoroutine(ReadyRoutine());
	public void StartRunState() => StartCoroutine(RunRoutine());

	private IEnumerator ReadyRoutine()
	{
		ReadyText.gameObject.SetActive(true);
		yield return null;
		ReadyText.gameObject.SetActive(false);
	}

	private IEnumerator RunRoutine()
	{
		ReadyText.gameObject.SetActive(true);
		yield return new WaitForSeconds(RunTextShowTime);
		ReadyText.gameObject.SetActive(false);
	}
}
