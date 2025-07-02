using System;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable, IDamageDealer
{
	public static Player Instance { get; private set; }
	public PlayerMover Mover { get; private set; }
	public PlayerItemChecker ItemChecker { get; private set; }

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
	public bool IsPlayerInvincible => _isInvincible;
	public bool IsPLayerShield => _isShield;

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

		_maxHealth = 100f;
		_curHealth = _maxHealth;

		_isInvincible = false;
		_isShield = false;
	}

	private void Death()
	{
		Debug.Log("Player Death. Game Over");
	}

	#endregion

	#region ---- Public Method ----
	public void OnTap(Vector2 pos)
	{
		//Attack();
	}

	public void TakeDamage(float amount)
	{
		if (_isInvincible) return;

		if(_isShield)
		{
			_isShield = false;
			OnShieldConsumed?.Invoke();
			return;
		}

		_curHealth -= amount;
		if (_curHealth <= 0)
			Death();
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
	#endregion

	#region ---- Setter ----
	internal void SetInvincible(bool on) => _isInvincible = on;
	internal void SetShieldOn() => _isShield = true;
	#endregion
}
