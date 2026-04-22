using System.Collections;
using UnityEngine;

public class PlayerEffectChecker : MonoBehaviour
{
	[Header("Shield Effect")]
	[SerializeField] private GameObject shieldEffect;

	[Header("Heal Effect")]
	[SerializeField] private GameObject healEffect;

	[Header("Booster Effect")]
	[SerializeField] private GameObject boosterEffect;
	[Header("Magnet Effect")]
	[SerializeField] private GameObject magnetEffect;
	private Coroutine magnetCoroutine;
	[SerializeField] private float floatAmplitude = 0.2f;
	[SerializeField] private float floatFrequency = 2f; 

	[Header("Elixir Effect")]
	[SerializeField] private GameObject elixirEffect;

	[Header("Coin Effect")]
	[SerializeField] private GameObject coinEffect;
	private Coroutine coinCoroutine;
	[SerializeField] private float jumpHeight = 1.5f;   // Ƣ����� ����
	[SerializeField] private float duration = 0.6f;     // ���� ���� �ð�
	[SerializeField] private float rotationSpeed = 720f; // �ʴ� ȸ�� �ӵ� (360�� * 2���� ��)

	#region --- Player Attack Effect ---
	public void AttackSwing()
	{

	}

	public void HitNormalAttack()
	{

	}

	public void HitFatalAttack()
	{

	}

	public void PlayerHitted()
	{

	}

	#endregion

	#region --- Item Effect ---
	public void ShieldEffect(bool isShieldOn)
	{
		if(isShieldOn)
		{
			shieldEffect.SetActive(true);
		}
		else
		{
			shieldEffect.SetActive(false);
		}
	}

	public void HealingEffect()
	{
		healEffect.SetActive(false);
		healEffect.SetActive(true);
	}

	public void DropCoinEffect()
	{
		coinEffect.SetActive(true);
		if(coinCoroutine != null)
		{
			StopCoroutine(coinCoroutine);
		}

		coinCoroutine = StartCoroutine(CoinAnimation());
	}

	private IEnumerator CoinAnimation()
	{
		Vector3 startPos = new Vector3(-0.05f, 0.53f, 0);
		coinEffect.transform.localPosition= startPos;
		coinEffect.transform.localRotation = Quaternion.identity;

		float elapsed = 0f;

		while (elapsed < duration)
		{
			elapsed += Time.deltaTime;
			float progress = elapsed / duration;

			float yOffset = Mathf.Sin(progress * Mathf.PI) * jumpHeight;
			transform.localPosition = startPos + new Vector3(0, yOffset, 0);

			transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

			yield return null;
		}

		coinEffect.transform.localPosition = startPos;
		coinEffect.transform.localRotation = Quaternion.identity;
		gameObject.SetActive(false);

		coinCoroutine = null;
	}

	public void BattleBoosterEffect(bool isBoosterOn)
	{
		if (isBoosterOn)
		{
			boosterEffect.SetActive(true);
		}
		else
		{
			boosterEffect.SetActive(false);
		}
	}

	public void CoinMagnetEffect(bool isMagnetOn)
	{
		if(isMagnetOn)
		{
			magnetEffect.SetActive(true);
			if (magnetCoroutine != null) StopCoroutine(magnetCoroutine);

			magnetCoroutine = StartCoroutine(FloatingRoutine());
		}
		else
		{
			magnetEffect.SetActive(false);
			if (magnetCoroutine != null)
			{
				StopCoroutine(magnetCoroutine);
				magnetCoroutine = null;
			}
		}
	}

	private IEnumerator FloatingRoutine()
	{
		Vector3 startPos = magnetEffect.transform.localPosition;

		while (true)
		{
			float newY = startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;

			magnetEffect.transform.localPosition = new Vector3(startPos.x, newY, startPos.z);

			yield return null;
		}
	}

	public void FatalElixirEffect(bool isElixirOn)
	{
		if(isElixirOn)
		{
			elixirEffect.SetActive(true);
		}
		else
		{
			elixirEffect.SetActive(false);
		}
	}
	#endregion
}
