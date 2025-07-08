using UnityEngine;

public class LazerBeam : MonoBehaviour
{
	private float beamDamage;
	private LazerTrap _trap;

	private void Start()
	{
		_trap = GetComponentInParent<LazerTrap>();
		if(_trap != null)
		{
			beamDamage = _trap.BeamDamage;
		}
	}

	private void OnTriggerEnter2D(Collider2D col)
	{
		if (!col.CompareTag("PLAYER")) return;

		if (col.TryGetComponent<Player>(out var player))
			player.TakeDamage(beamDamage);
	}
}
