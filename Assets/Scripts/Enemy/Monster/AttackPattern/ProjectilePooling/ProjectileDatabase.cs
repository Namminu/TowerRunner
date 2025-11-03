using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public enum ProjectileType
{
    Arrow,
    LightCross,
    DarkCross,
}

[CreateAssetMenu(fileName = "ProjectileDatabase", menuName = "Scriptable Objects/ProjectileDatabase")]
public class ProjectileDatabase : ScriptableObject
{
    [Serializable]
    public struct ProjectileEntry
    {
        public ProjectileType type;
        public AssetReferenceGameObject prefabRef;
        public int poolSize;
    }

    public List<ProjectileEntry> entries = new();

    public ProjectileEntry? FindProjectile(ProjectileType type)
    {
        for(int i = 0; i< entries.Count; i++)
        {
            if(entries[i].type == type)
				return entries[i];
		}
        return null;
	}
}
