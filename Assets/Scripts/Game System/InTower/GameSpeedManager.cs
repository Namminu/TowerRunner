using UnityEngine;

public class GameSpeedManager : MonoBehaviour
{
	public static GameSpeedManager Instance { get; private set; }

	public float SpeedMultiplier { get; private set; } = 1f;

	[Tooltip("speed increase per second during combat")]
	[SerializeField, Range(0.01f, 1f)]
	private float speedAccelrator = 0.01f;

	private void Awake()
	{
		Instance = this;
	}

	private void Update()
	{
		SpeedMultiplier = 1f + Time.timeSinceLevelLoad * speedAccelrator;
	}
}
