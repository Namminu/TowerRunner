using UnityEngine;

public class Player : MonoBehaviour
{
	public static Player Instance {  get; private set; }
	public PlayerMover Mover { get; private set; }

	#region ---- Members ----
	[SerializeField, Tooltip("플레이어 이동 속도"), Range(0, 10)]
	private float playerSpeed = 5f;

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
		
	}

	private void Attack()
	{

	}


	#endregion

	#region ---- Public Method ----
	public void OnTap(Vector2 pos)
	{
		Attack();
	}


	#endregion
}
