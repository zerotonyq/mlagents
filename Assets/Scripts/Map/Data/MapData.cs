using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

namespace Map.Data
{
    [CreateAssetMenu(menuName = "Gameplay/Map/MapData", fileName = "Map")]
    public class MapData : ScriptableObject
    {
        public float ChunkBoundLength = 20f;
        //Square MAP
        public ChunkMatrix chunks = new();
    }

    [Serializable]
    public class ChunkRow
    {
        public List<AssetReference> X = new();
    }
    [Serializable]
    public class ChunkMatrix
    {
        public List<ChunkRow> Y = new();
    }
}