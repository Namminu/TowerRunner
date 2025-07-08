using System.Collections;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class LazerTrap : BaseObject
{
	[Header("Before Lazer Attack")]
	[SerializeField]
	private GameObject WarningZone;
	[SerializeField, Tooltip("Warning Time")]
    private float warningTime;
	[SerializeField, Tooltip("Warning Zone Blink Interval")]
	private float onInterval;
	[SerializeField]
	private float offInterval;

	[Header("Attack Routine")]
	[SerializeField]
	private GameObject BeamZone;
    [SerializeField, Tooltip("Lazer Beam Attack Damage, Not Lazer Hit Damage")]
    private float beamDamage;
	public float BeamDamage => beamDamage;
	[SerializeField, Tooltip("Lazer Beam Dealing Duration")]
	private float beamDuration;

	[Header("Launch Loop")]
	[SerializeField]
	private float launchInterval;

	private void Start()
	{
		StartCoroutine(LaunchLoop());
	}

	private IEnumerator LaunchLoop()
	{
		while (true)
		{
			float speedMul = GameSpeedManager.Instance.SpeedMultiplier;
			float waitTime = launchInterval / speedMul;

			StartCoroutine(LaunchBeam());
			yield return new WaitForSeconds(waitTime);
		}
	}

	private IEnumerator LaunchBeam()
	{
		float elapsed = 0f;
		WarningZone.SetActive(false);
		BeamZone.SetActive(false);

		while (elapsed < warningTime)
		{
			WarningZone.SetActive(true);
			yield return new WaitForSeconds(onInterval);
			elapsed += onInterval;
			if (elapsed >= warningTime) break;

			WarningZone.SetActive(false);
			yield return new WaitForSeconds(offInterval);
			elapsed += offInterval;
		}
		WarningZone.SetActive(false);

		BeamZone.SetActive(true);
		yield return new WaitForSeconds(beamDuration);
		BeamZone.SetActive(false);
	}

	public override void OnSpawn()
	{
		base.OnSpawn();
		var pos = transform.position;
		pos.x = 0f;
		transform.position = pos;
	}

	public override void OnDespawn()
	{
		base.OnDespawn();
		StopAllCoroutines();
	}
}
