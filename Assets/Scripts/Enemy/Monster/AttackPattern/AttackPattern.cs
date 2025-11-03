using System.Collections;
using UnityEngine;

public abstract class AttackPattern : MonoBehaviour
{
    [SerializeField]
    private float attackCoolDown;
    public float AttackCoolDown => attackCoolDown;

    public void ExecuteAttack(float dmg, float delayTime)
        => StartCoroutine(AttackRoutine(dmg, delayTime));

    protected abstract IEnumerator AttackRoutine(float dmg, float delayTime);
}
