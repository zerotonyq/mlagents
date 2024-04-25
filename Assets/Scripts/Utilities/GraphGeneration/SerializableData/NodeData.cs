using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace TerrainGeneration.Data
{
    [Serializable]
    public class NodeData : Utilities.SaveLoad.Data
    {
        private float3 _position;


        private List<float3> _neighborsPositions;

        public NodeData(Vector3 pos, List<float3> neighborsPositions)
        {
            _position =new float3(pos.x, pos.y, pos.z);
            _neighborsPositions = neighborsPositions;
        }

        public float3 Position => _position;

        public List<float3> NeighborsPositions => _neighborsPositions;
    }
}