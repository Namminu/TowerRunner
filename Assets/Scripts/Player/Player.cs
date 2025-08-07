using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnforceType
{
	Health = 0,
	Fatal = 1,
	Power = 2,
	Range = 3
}

public class Player : MonoBehaviour, IDamageable, IDamageDealer
{
	public static Player Instance { get; private set; }
	public PlayerMover Mover { get; private set; }
	public PlayerItemChecker ItemChecker { get; private set; }
	public PlayerEffectChecker EffectChecker { get; private set; }

	#region ---- Members ----
	[Header("Player State Stats")]
	[SerializeField, Tooltip("Player SideMove Speed"), Range(50, 100)]
	private float playerSpeed = 50f;

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

	[SerializeField, Tooltip("Player Attack Radius"), Range(1, 5)]
	private float attackRadius = 1f;
	[SerializeField, Tooltip("Player Attack Circle Angle"), Range(90f, 180f)]
	private float attackAngle = 130f;
	[SerializeField]
	private Vector2 attackOffset = Vector2.zero;
	[SerializeField]
	private LayerMask attackTargetLayer;

	[Header("Player Finances")]
	[SerializeField, Tooltip("Player Gold Count")]
	private int _gold;
	public int PlayerGold
	{
		get => _gold;
		set
		{
			_gold += value;
			GameEvents.RaiseGoldChanged(_gold);
		}
	}

	private List<int> enforceLevels = new List<int>();

	private bool _isInvincible;
	private bool _isShield;
	private bool _isDamageCoolDown;

	public bool IsPlayerInvincible => _isInvincible;
	public bool IsPLayerShield => _isShield;
	public bool IsDamageCoolDown => _isDamageCoolDown;

	public event Action OnShieldConsumed;
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

		ItemChecker = GetComponent<PlayerItemChecker>();
		EffectChecker = GetComponent<PlayerEffectChecker>();
		_spriteRenderer = GetComponent<SpriteRenderer>();

		_maxHealth = 100f;
		_curHealth = _maxHealth;

		_isInvincible = false;
		_isShield = false;
		_isDamageCoolDown = false;
	}

	private void Start()
	{
		ApplySavedPlayerData();
	}

	private void ApplySavedPlayerData()
	{
		_gold = SaveService.Current.gold;
		GameEvents.RaiseGoldChanged(_gold);

		for(int idx = 0; idx < SaveService.Current.upgrades.Count; idx++) 
		{
			var u = SaveService.Current.upgrades[idx];
			float value = EnforceService.GetValue(idx, u.level);
			ApplyUpgrade((EnforceType)idx, value);
			enforceLevels[idx] = u.level;
		}
		_curHealth = Mathf.Min(_curHealth, _maxHealth);
	}

	private void Death()
	{
		Debug.Log("Player Death. Game Over");
	}

	private IEnumerator DamagedInvincible()
	{
		_isDamageCoolDown = true;

		float elapsed = 0f;
		bool visible = true;

		while(elapsed < damagedInvincibleTime)
		{
			visible = !visible;
			_spriteRenderer.enabled = visible;
			yield return new WaitForSeconds(blinkInterval);
			elapsed += blinkInterval;
		}

		_spriteRenderer.enabled = true;
		_isDamageCoolDown = false;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Vector3 center = (Vector2)transform.position + attackOffset;
		Gizmos.DrawWireSphere(center, attackRadius);

		float halfArc = attackAngle * 0.5f;
		Vector3 fwd = transform.up * attackRadius;
		Quaternion leftRot = Quaternion.Euler(0, 0, halfArc);
		Quaternion rightRot = Quaternion.Euler(0, 0,-halfArc);
		Gizmos.DrawLine(center, center + leftRot * fwd);
		Gizmos.DrawLine(center, center + rightRot * fwd);
	}

	#endregion

	#region ---- Public Method ----
	public void OnTap(Vector2 pos)
	{
		EffectChecker.AttackSwing();

		Vector2 center = (Vector2)transform.position + attackOffset;
		Collider2D[] hits = Physics2D.OverlapCircleAll(center, attackRadius, attackTargetLayer);

		float halfArc = attackAngle * 0.5f;
		Vector2 forward = transform.up;

		foreach(var col in hits)
		{
			Vector2 dir = (col.transform.position - (Vector3)center).normalized;
			float angle = Vector2.Angle(forward, dir);
			if(angle <= halfArc)
			{
				if(col.TryGetComponent<IDamageable>(out var target))
				{
					DealDamage(target);
				}
			}
		}
	}

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
		EffectChecker.PlayerHitted();
		if (_curHealth <= 0)
		{
			Death();
			return;
		}		

		StartCoroutine(DamagedInvincible());
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

		_curHealth = 0;
		Death();
	}
	#endregion

	#region ---- Enforcing ----
	public int GetEnforceLevel(int idx)
	{
		return enforceLevels[idx];
	}

	public void SetEnforceLevel(int idx, int level)
	{
		enforceLevels[idx] = level;
	}

	public bool SpendGold(int amount)
	{
		if (_gold < amount) return false;
		PlayerGold = -amount;
		return true;
	}

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
			case EnforceType.Range:
				attackRadius = value;		break;
			default:
				Debug.LogWarning($"Unknown Upgrade Type : {type}");
				break;
		}
	}

	#endregion

	#region ---- Setter ----
	internal void SetInvincible(bool on) => _isInvincible = on;
	internal void SetShieldOn() => _isShield = true;
	#endregion
}
