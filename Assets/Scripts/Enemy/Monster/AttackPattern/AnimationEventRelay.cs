using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
	public BaseMonster monster;

	public void OnAttackAnimationEnd()
	{
		monster.OnAttackAnimationEnd();
	}

	/// <summary>
	/// Orc 전용 번개 프리팹 생성 트리거
	/// </summary>
	public void OnTunderAnimationTrigged()
	{
		if(monster is Orc orc)
		{
			orc.OnTunderAnimationTrigged();
		}
	}

	/// <summary>
	/// DarkAngle 전용 낫 프리팹 생성 트리거
	/// </summary>
	public void OnSickleAnimationTrigged()
	{
		if (monster is DarkAngle dg)
		{
			dg.OnSickleAnimationTrigged();
		}
	}
}
