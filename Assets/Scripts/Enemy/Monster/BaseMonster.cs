using System.Collections;
using UnityEngine;

public abstract class BaseMonster : BaseEnemy, IDamageable
{
	[Header("Monster Stats")]
	[SerializeField]
	private float _attackDamage;
	public float AttackDamage => _attackDamage;

	[SerializeField]
	private float enemyMaxHealth;
	private float enemyCurHealth;

	public Animator Animator { get; private set; }
	private SpriteRenderer sr;

	[Header("Monster Attacks")]
	[SerializeField] private float _attackDelay;
	protected AttackPattern _attackPattern;
	private Coroutine _attackRoutine;

	private bool _isDead = false;
	private ObjectMover _objectMover;

	protected override void Awake()
	{
		base.Awake();
		_objectMover = GetComponent<ObjectMover>();

		_attackPattern = GetComponent<AttackPattern>();
		Animator = GetComponentInChildren<Animator>();
		sr = GetComponentInChildren<SpriteRenderer>();
	}

	protected virtual void OnEnable()
	{
		_isDead = false;
		enemyCurHealth = enemyMaxHealth;
		_attackRoutine = StartCoroutine(AttackRoutine());

		GameStateManager.OnStateChanged += HandleGameStateChanged;
	}

	protected virtual void OnDisable()
	{
		if (_attackRoutine != null)
		{
			StopCoroutine(_attackRoutine);
			_attackRoutine = null;
		}
		_isDead = true;

		GameStateManager.OnStateChanged -= HandleGameStateChanged;
	}

	private void HandleGameStateChanged(GameStateManager.GameState curState)
	{
		switch (curState)
		{
			case GameStateManager.GameState.GameOver:
				if (_attackRoutine != null)
				{
					StopCoroutine(_attackRoutine);
					_attackRoutine = null;
				}
				break;
		}
	}

	public override void OnSpawn()
	{
		base.OnSpawn();

		//if (GetComponentInChildren<Animator>() is Animator ani)
		//	if(ani) ani.Play("Run");
	}

	protected void Death()
	{
		_isDead = true;
		StartCoroutine(DeathRoutine());
	}

	private IEnumerator DeathRoutine()
	{
		_collider.enabled = false;
		_objectMover.PauseMovement();
		Animator.SetBool("IsDeath", true);

		ScoreManager.Instance.RegisterMonsterKill();

		yield return new WaitForSeconds(0.5f);

		EnemyPoolingManager.Instance.Despawn(this);

		ItemData dropItem = ItemDatabase.GetRandomDrop();
		if (dropItem != null)
		{
			ItemPoolingManager.Instance.Spawn(dropItem, transform.position);
		}
	}

	public virtual void TakeDamage(float amount)
	{
		enemyCurHealth -= amount;
		if (enemyCurHealth <= 0f)
			Death();
		else StartCoroutine(BlinkRoutine());

		Debug.Log(name + "Take Damage : " + amount);

		AudioManager.Instance.PlaySound(AudioID.EnemyHit);
	}

	public virtual void Attack()
	{
		_objectMover.PauseMovement();
		Animator.SetTrigger("IsAttack");
	
		_attackPattern.ExecuteAttack(_attackDamage, _attackDelay);
	}

	private IEnumerator AttackRoutine()
	{
		WaitForSeconds waitTime = new(_attackPattern.AttackCoolDown);
		while (!_isDead)
		{
			yield return waitTime;
			Attack();
		}
	}

	private IEnumerator BlinkRoutine()
	{
		float waitTimePerState = 0.8f / 6;
		WaitForSeconds waitTime = new(waitTimePerState);

		for (int i = 0; i < 3; i++)
		{
			// True -> False
			sr.enabled = false;
			yield return waitTime;

			// False -> True
			sr.enabled = true;
			yield return waitTime;
		}

		sr.enabled = true;
	}

	public virtual void OnAttackAnimationEnd()
	{
		_objectMover.ResumeMovement();
	}
}