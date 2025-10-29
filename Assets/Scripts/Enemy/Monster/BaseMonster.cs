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

	protected AttackPattern _attackPattern;

	private Coroutine _attackRoutine;

	protected virtual void Awake()
	{
		enemyCurHealth = enemyMaxHealth;

		_attackPattern = GetComponent<AttackPattern>();
		Animator = GetComponentInChildren<Animator>();
		sr = GetComponentInChildren<SpriteRenderer>();
	}

	protected virtual void OnEnable()
	{
		_attackRoutine = StartCoroutine(AttackRoutine());
	}

	protected virtual void OnDisable()
	{
		if (_attackRoutine != null)
		{
			StopCoroutine(_attackRoutine);
			_attackRoutine = null;
		}
	}

	protected void Death()
	{
		ItemData dropItem = ItemDatabase.GetRandomDrop();
		if (dropItem != null)
		{
			ItemPoolingManager.Instance.Spawn(dropItem,transform.position);
		}
		EnemyPoolingManager.Instance.Despawn(this);
	}

	public virtual void TakeDamage(float amount)
	{
		enemyCurHealth -= amount;
		if (enemyCurHealth <= 0f)
			Death();
		else StartCoroutine(BlinkRoutine());

		Debug.Log(name + "Take Damage : " + amount);
	}

	public virtual void Attack()
	{
		var mover = GetComponent<ObjectMover>();
		mover.PauseMovement();
		Animator.SetTrigger("IsAttack");
	
		_attackPattern.ExecuteAttack(_attackDamage);
	}

	private IEnumerator AttackRoutine()
	{
		while (true)
		{
			yield return new WaitForSeconds(_attackPattern.AttackCoolDown);
			Attack();
		}
	}

	private IEnumerator BlinkRoutine()
	{
		for(int i = 0; i < 3; i++)
		{
			sr.enabled = false;
			yield return new WaitForSeconds(0.3f);
			sr.enabled = true;
			yield return new WaitForSeconds(0.3f);
		}
		sr.enabled = true;
	}

	public virtual void OnAttackAnimationEnd()
	{
		var mover = GetComponent<ObjectMover>();
		mover.ResumeMovement();
	}
}