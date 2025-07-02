using System.Collections.Generic;
using UnityEngine;

public class ItemPoolingManager : MonoBehaviour
{
	public static ItemPoolingManager Instance { get; private set; }

	[SerializeField]
	ItemDatabase itemDatabase;
	//Dictionary<> itemPools;

	private void Awake()
	{
		
	}
}