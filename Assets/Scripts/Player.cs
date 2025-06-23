using UnityEngine;

public class Player : MonoBehaviour, IDamageable, IDamageDealer
{
	public static Player Instance {  get; private set; }
	public PlayerMover Mover { get; private set; }

	#region ---- Members ----
	[SerializeField, Tooltip("�÷��̾� �̵� �ӵ�"), Range(50, 100)]
	private float playerSpeed = 50f;

	[SerializeField, Tooltip("�÷��̾� �ִ� ü��")]
	private float playerMaxHealth = 100f;
	private float playerCurHealth;
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

		playerCurHealth = playerMaxHealth;
	}

	private void Attack()
	{
		Debug.Log("Player Attack!");
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
		playerCurHealth -= amount;
		if (playerCurHealth <= 0)
			Death();
	}

	public void DealDamage(IDamageable target)
	{
		
	}


	#endregion
}
