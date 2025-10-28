using UnityEngine;

public abstract class AttackPattern : MonoBehaviour
{
    [SerializeField]
    private float attackCoolDown;
    public float AttackCoolDown => attackCoolDown;

    public abstract void ExecuteAttack(float dmg);
}
