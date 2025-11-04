using UnityEngine;

public class Angle : BaseMonster
{
	[SerializeField]
	private Transform[] shotPoint;

	public void Start()
	{
		if (_attackPattern != null && _attackPattern is ShotAttack shotAtk)
		{
			shotAtk.SetShotPoint(shotPoint);
		}
	}
}
