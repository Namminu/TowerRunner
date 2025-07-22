using System;
using System.Collections;
using UnityEngine;

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
	private int _fatalRate;
	public int PlayerFatalRate
	{
		get => _fatalRate;
		set
		{
			_fatalRate = Mathf.Clamp(value, 0, 100);
		}
	}

	[SerializeField, Tooltip("Player Attack Power"), Range(0, 10)]
	private float playerPower = 1f;

	[SerializeField, Tooltip("Player Attack Radius"), Range(0, 10)]
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
		}
	}

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
			damage *= 2;
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

	#region ---- Setter ----
	internal void SetInvincible(bool on) => _isInvincible = on;
	internal void SetShieldOn() => _isShield = true;
	#endregion
}
