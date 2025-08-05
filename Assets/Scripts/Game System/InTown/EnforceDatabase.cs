using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnforceDatabase", menuName = "Scriptable Objects/EnforceDatabase")]
public class EnforceDatabase : ScriptableObject
{
    [Serializable]
    public class EnforceData
    {
        [Header("UI")]
        public string displayName;
        [TextArea] public string description;

        [Header("Setting")]
        public int initLevel = 1;
        public int initCost = 100;
        [Range(1f, 3f)]
        public float costMultiplier = 1.1f;
        public int maxLevel = 99;

        [Header("Value")]
        public float initValue = 100f;
        public float valueIncrement = 10f;
    }

    public List<EnforceData> enforceDB = new List<EnforceData>();
}
