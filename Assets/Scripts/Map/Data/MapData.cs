using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Map.Data
{
    [CreateAssetMenu(menuName = "Gameplay/Map/MapData", fileName = "Map")]
    public class MapData : ScriptableObject
    {
        public float ChunkBoundLength;
        public ChunkMatrix chunks = new();
    }

    [Serializable]
    public class ChunkRow
    {
        public List<AssetReference> Y = new();
    }
    [Serializable]
    public class ChunkMatrix
    {
        public List<ChunkRow> X = new();
    }
}