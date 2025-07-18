using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public interface IInitializable
{
	void Init();
}

public class ManagersInitializer : MonoBehaviour
{
	[Header("Common Managers")]
	[SerializeField] private List<AssetReferenceGameObject> commonManagerRefs;

	[Header("Each Scene Managers")]
	[SerializeField] private AssetReferenceGameObject MainManagerRef;
	[SerializeField] private AssetReferenceGameObject TownManagerRef;
	[SerializeField] private AssetReferenceGameObject TowerManagerRef;


}
