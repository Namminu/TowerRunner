using UnityEngine;

public class Player : MonoBehaviour, IDamageable, IDamageDealer
{
	public static Player Instance { get; private set; }
	public PlayerMover Mover { get; private set; }

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
	private int playerFatalRate = 0;
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

	private bool isPlayerInvincible;
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
		_maxHealth = 100f;
		_curHealth = _maxHealth;
		isPlayerInvincible = false;
	}

	private void Attack()
	{
		if(Random.Range(0, 100) < playerFatalRate) /* Fatal Attack Called */
		{
			Debug.Log("Player Fatal Attack!");

		}
		else								       /* Normal Attack Called */
		{
			Debug.Log("Player Attack!");

		}
	}

	private void Death()
	{
		Debug.Log("Player Death. Game Over");
	}

	#endregion

	#region ---- Public Method ----
	public void OnTap(Vector2 pos)
	{
		Attack();
	}

	public void TakeDamage(int amount)
	{
		if (isPlayerInvincible) return;

		_curHealth -= amount;
		if (_curHealth <= 0)
			Death();
	}

	public void DealDamage(IDamageable target)
	{
		
	}

	public void Heal(float amount)
	{
		Debug.Log("Player Heal! " + amount);
		_curHealth += amount;
	}

	public void ApplyBattleBooster(float duration)
	{
		isPlayerInvincible = true;
	}

	public void RemoveBattleBooster()
	{
		isPlayerInvincible = false;
	}

	#endregion
}
