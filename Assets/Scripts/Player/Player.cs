using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEditor.TextCore.Text;
using UnityEngine;

public enum EnforceType
{
	Health = 0,
	Fatal = 1,
	Power = 2,
	Speed = 3
}

public class Player : MonoBehaviour, IDamageable, IDamageDealer
{
	public static Player Instance { get; private set; }
	public PlayerMover Mover { get; private set; }
	public PlayerItemChecker ItemChecker { get; private set; }
	public PlayerEffectChecker EffectChecker { get; private set; }

	public Animator Ani { get; private set; }

	#region ---- Members ----
	[Header("Player State Stats")]
	[SerializeField, Tooltip("Player SideMove Speed"), Range(50, 100)]
	private float playerSpeed = 50f;

	[SerializeField, Tooltip("Player Run Speed")]
	private float playerRunSpeed = 1f;
	public float PlayerRunSpeed => playerRunSpeed;

	[SerializeField, Tooltip("Player Max Health")]
	private float _maxHealth;
	public float PlayerMaxHealth => _maxHealth;
	
	private float _curHealth;
	public float PlayerCurHealth
	{
		get => _curHealth;
		set
		{
			_curHealth = Mathf.Clamp(value, 0f, PlayerMaxHealth);
		}
	}
	[SerializeField, Tooltip("Auto Decrease Health Amount")]
	private float autoDecAmount = 4f;

	[SerializeField, Tooltip("Invincible Time for Take Damage")]
	private float damagedInvincibleTime;
	[SerializeField, Tooltip("Blink Effect Time for Take Damage")]
	private float blinkInterval;
	private SpriteRenderer _spriteRenderer;

	[Header("Player Power Stats")]
	[SerializeField, Tooltip("Player Fatal Attack Rate"), Range(0, 100)]
	private int _fatalRate = 25;
	public int PlayerFatalRate
	{
		get => _fatalRate;
		set
		{
			_fatalRate = Mathf.Clamp(value, 0, 100);
		}
	}

	[SerializeField, Tooltip("Player Fatal Attack Damage"), Range(0, 400)]
	private float _fatalIncreaseDamage = 10f;
	public float PlayerFatalDamage
	{
		get => _fatalIncreaseDamage;
		set
		{
			_fatalIncreaseDamage = Mathf.Clamp(value, 0f, 400f);
		}
	}

	[SerializeField, Tooltip("Player Attack Power"), Range(0, 10)]
	private float playerPower = 1f;

	[Header("Attack")]
	//[SerializeField, Tooltip("Player Attack Radius"), Range(1, 5)]
	//private float attackRadius = 1f;
	//[SerializeField, Tooltip("Player Attack Circle Angle"), Range(90f, 180f)]
	//private float attackAngle = 130f;
	//[SerializeField]
	//private Vector2 attackOffset = Vector2.zero;
	//[SerializeField]
	//private LayerMask attackTargetLayer;
	private bool isAttacking = false;
	[SerializeField]
	private BoxCollider2D AttackRange;
	private ContactFilter2D contactFilter;
	private Collider2D[] results = new Collider2D[8];

	public int PlayerGold => EconomyService.Gold;

	private bool _isInvincible;
	private bool _isShield;
	private bool _isDamageCoolDown;

	public bool IsPlayerInvincible => _isInvincible;
	public bool IsPlayerShield => _isShield;
	public bool IsDamageCoolDown => _isDamageCoolDown;

	public event Action OnShieldConsumed;

	public event Action<float> OnHealthChanged;

	public event Action _runUnsub;
	private Coroutine autoHealthDecrease;
	#endregion


	#region ---- Private Method ----

	private void Awake()
	{
		if(Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);

		Mover = GetComponent<PlayerMover>();
		if(Mover != null)
		{
			Mover.SetSpeed(playerSpeed);
		}

		ItemChecker = GetComponentInChildren<PlayerItemChecker>();
		EffectChecker = GetComponentInChildren<PlayerEffectChecker>();
		_spriteRenderer = GetComponentInChildren<SpriteRenderer>();

		ApplySavedPlayerData();
		_curHealth = _maxHealth;

		_isInvincible = false;
		_isShield = false;
		_isDamageCoolDown = false;

		Ani = GetComponentInChildren<Animator>();

		if (AttackRange == null)
			AttackRange = GetComponentInChildren<BoxCollider2D>();
		contactFilter = new ContactFilter2D();
		contactFilter.SetLayerMask(LayerMask.GetMask("Monster"));
		contactFilter.useLayerMask = true;
		contactFilter.useTriggers = true;
	}

	private void Start()
	{
		_runUnsub = GameBus.Subscribe<RunSignal>(StartAutoDecrease);

		GameEvents.OnBattleEnded += StopAutoDecrease;
	}

	private void ApplySavedPlayerData()
	{
		if(SaveService.Current != null)
		{
			for (int idx = 0; idx < SaveService.Current.upgrades.Count; idx++)
			{
				int level = UpgradeService.GetLevel(idx);
				UpgradeService.SetLevel(idx, level, applyToPlayer: true);
			}
		}
	}

	private void Death()
	{
		Debug.Log("Player Death. Game Over");

		GameEvents.RaiseBattleEnd();
		Ani.SetBool("IsDeath", true);
	}

	//private IEnumerator DamagedInvincible()
	//{
	//	_isDamageCoolDown = true;

	//	float elapsed = 0f;
	//	bool visible = true;
	//	WaitForSeconds waitTime = new(blinkInterval);

	//	while (elapsed < damagedInvincibleTime)
	//	{
	//		visible = !visible;
	//		_spriteRenderer.enabled = visible;
	//		yield return waitTime;
	//		elapsed += blinkInterval;
	//	}

	//	_spriteRenderer.enabled = true;
	//	_isDamageCoolDown = false;
	//}

	private void StartAutoDecrease(RunSignal sig)
	{
		autoHealthDecrease = StartCoroutine(AutoHealthDecreaseRoutine());
	}

	private IEnumerator AutoHealthDecreaseRoutine()
	{
		WaitForSeconds waitTime = new(1f);

		while(true)
		{
			yield return waitTime;

			_curHealth -= autoDecAmount;

			float healthRatio = _curHealth / _maxHealth;
			OnHealthChanged?.Invoke(healthRatio);

			if (_curHealth <= 0)
			{
				Death();
				yield break;
			}
		}
	}

	private void StopAutoDecrease()
	{
		if(autoHealthDecrease != null)
		{
			StopCoroutine(autoHealthDecrease);
			autoHealthDecrease = null;
		}
	}

	private IEnumerator DamagedInvincible()
	{
		_isDamageCoolDown = true;

		float waitTimePerState = damagedInvincibleTime / (blinkInterval * 2);
		WaitForSeconds waitTime = new(waitTimePerState);

		for(int i = 0; i<blinkInterval; i++)
		{
			// True -> False
			_spriteRenderer.enabled = false;
			yield return waitTime;

			// False -> True
			_spriteRenderer.enabled = true;
			yield return waitTime;
		}

		_spriteRenderer.enabled = true;
		_isDamageCoolDown = false;
	}

	#endregion

	#region ---- Public Method ----

	public void OnTap(Vector2 pos)
	{
		if (isAttacking) return;

		isAttacking = true;

		EffectChecker.AttackSwing();

		//Vector2 center = (Vector2)transform.position + attackOffset;
		//Collider2D[] hits = Physics2D.OverlapCircleAll(center, attackRadius, attackTargetLayer);

		//float halfArc = attackAngle * 0.5f;
		//Vector2 forward = transform.up;

		//foreach(var col in hits)
		//{
		//	Vector2 dir = (col.transform.position - (Vector3)center).normalized;
		//	float angle = Vector2.Angle(forward, dir);
		//	if(angle <= halfArc)
		//	{
		//		if(col.TryGetComponent<IDamageable>(out var target))
		//		{
		//			DealDamage(target);
		//		}
		//	}
		//}

		Vector2 worldPos = (Vector2)AttackRange.transform.position +
							(Vector2)(AttackRange.transform.rotation * AttackRange.offset);

		int hitCount = Physics2D.OverlapBox(
			worldPos, AttackRange.size, AttackRange.transform.eulerAngles.z,
			contactFilter, results);

		for(int i = 0; i < hitCount; i++)
		{
			var col = results[i];
			IDamageable damageable = col.GetComponentInParent<IDamageable>();
			if(damageable != null)
			{
				DealDamage(damageable);
			}
		}

		Ani.SetTrigger("IsAttack");
	}

	private void OnDrawGizmosSelected()
	{
		if (AttackRange is BoxCollider2D box)
		{
			Gizmos.color = Color.red;
			// 몬스터의 현재 위치 + 회전이 반영된 오프셋
			Vector2 worldPos = (Vector2)AttackRange.transform.position +
							   (Vector2)(AttackRange.transform.rotation * box.offset);

			Gizmos.matrix = Matrix4x4.TRS(worldPos, AttackRange.transform.rotation, Vector3.one);
			Gizmos.DrawWireCube(Vector3.zero, box.size);
		}
	}

	public void EndAttack() => isAttacking = false;

	public void TakeDamage(float amount)
	{
		if (_isInvincible || _isDamageCoolDown) return;

		if(_isShield)
		{
			_isShield = false;
			OnShieldConsumed?.Invoke();
			return;
		}

		_curHealth -= amount;

		float healthRatio = _curHealth / _maxHealth;
		OnHealthChanged?.Invoke(healthRatio);

		if (_curHealth <= 0)
		{
			Death();
			return;
		}
		else
		{
			EffectChecker.PlayerHitted();
			StartCoroutine(DamagedInvincible());

			ScoreManager.Instance.RegisterPlayerHit();
		}
	}

	public void DealDamage(IDamageable target)
	{
		float damage = playerPower;
		if (UnityEngine.Random.Range(0, 100) < _fatalRate) /* Fatal Attack Called */
		{
			Debug.Log("Player Fatal Attack!");
			damage += _fatalIncreaseDamage;
			target.TakeDamage(damage);
		}
		else                                       /* Normal Attack Called */
		{
			Debug.Log("Player Attack!");
			target.TakeDamage(damage);
		}
	}

	public void ImmediateDeath()
	{
		if (_isInvincible || _isDamageCoolDown) return;

		ScoreManager.Instance.RegisterPlayerHit();

		_curHealth = 0;
		Death();
	}
	#endregion

	#region ---- Enforcing ----
	public int GetEnforceLevel(int idx)
	{
		return UpgradeService.GetLevel(idx);
	}

	public void SetEnforceLevel(int idx, int level)
	{
		UpgradeService.SetLevel(idx, level);
	}

	public bool SpendGold(int amount)
		=> EconomyService.TrySpendGold(amount);

	public void AddGold(int amount)
		=> EconomyService.AddGold(amount);

	public void ApplyUpgrade(EnforceType type, float value)
	{
		switch (type)
		{
			case EnforceType.Health:
				_maxHealth = value;			break;
			case EnforceType.Fatal:
				PlayerFatalDamage = value;	break;
			case EnforceType.Power:
				playerPower = value;		break;
			case EnforceType.Speed:
				playerRunSpeed = value;		break;
			default:
				Debug.LogWarning($"Unknown Upgrade Type : {type}");
				break;
		}
	}

	#endregion

	#region ---- Setter ----
	internal void SetInvincible(bool on) => _isInvincible = on;
	internal void SetShieldOn() => _isShield = true;

	internal void ResetInTower(Vector3 resetPosition)
	{
		// Tower Scene 위치 초기화
		transform.position = resetPosition;

		/* 스탯 초기화 */
		ApplySavedPlayerData();
		// 체력
		_curHealth = _maxHealth;
		// 스탯
		_isInvincible = false;
		_isShield = false;
		_isDamageCoolDown = false;

		/* 아이템 사용 여부 초기화 */
		if (ItemChecker != null)
		{
			ItemChecker.ResetAllItemApply();
		}
		else Debug.Log("Player ItemChecker Script Null Error");
	}

	internal void SetPlayerObjectState(bool isPlayerActive)
		=> gameObject.SetActive(isPlayerActive);
	#endregion

	private void OnDestroy()
	{
		_runUnsub?.Invoke();

		GameEvents.OnBattleEnded -= StopAutoDecrease;
	}
}
