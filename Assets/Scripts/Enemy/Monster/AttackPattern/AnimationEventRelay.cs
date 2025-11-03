using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
	public BaseMonster monster;

	public void OnAttackAnimationEnd()
	{
		monster.OnAttackAnimationEnd();
	}

	public void OnTunderAnimationTrigged()
	{
		if(monster is Orc orc)
		{
			orc.OnTunderAnimationTrigged();
		}
	}
}
