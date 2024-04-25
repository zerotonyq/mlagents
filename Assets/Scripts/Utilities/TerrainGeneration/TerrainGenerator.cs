using System;
using System.Collections;
using System.Collections.Generic;
using Autodesk.Fbx;
using TerrainGeneration.Converter;
using Unity.VisualScripting;
using UnityEditor.Formats.Fbx.Exporter;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities.SaveLoad;
using TerrainData = TerrainGeneration.SerializationData.TerrainData;

namespace TerrainGeneration
{
    [RequireComponent(typeof(MeshFilter))]
    public class TerrainGenerator : MonoBehaviour
    {
        private Mesh mesh;

        [SerializeField] private int terrainSizeX;
        [SerializeField] private int terrainSizeY;
        [SerializeField] private int chunkSize;
        [Space(10f)]
        [SerializeField] private float noiseFrequencyX1;
        [SerializeField] private float noiseFrequencyZ1;
        [SerializeField] private float noiseAmplitude1;
        [SerializeField] private float noiseOffsetX1;
        [SerializeField] private float noiseOffsetY1;
        [Space(5f)]
        [SerializeField] private float noiseFrequencyX2;
        [SerializeField] private float noiseFrequencyZ2;
        [SerializeField] private float noiAmplitude2;
        [SerializeField] private float noiseOffsetX2;
        [SerializeField] private float noiseOffsetY2;
        [Space(5f)]
        [SerializeField] private float noiseStrength;
        
        
        

        private Vector3[] _vertices;

        private int[] _triangles;

        public Action<Vector3[], int, int> TerrainGenerated;

        public Mesh Mesh => mesh;

        private void Start()
        {
            InitMesh();
            GenerateMesh();
        }

        private void InitMesh()
        {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
        }

        [ContextMenu("GenerateTerrainMesh")]
        private void GenerateMesh() => StartCoroutine(GenerateMeshCoroutine());

        private IEnumerator GenerateMeshCoroutine()
        {
            _vertices = new Vector3[(terrainSizeX + 1) * (terrainSizeY + 1)];

            for (int i = 0, z = 0; z <= terrainSizeY; z++)
            {
                for (int x = 0; x <= terrainSizeX; x++)
                {
                    float y = (Mathf.PerlinNoise(x * noiseFrequencyX1 + noiseOffsetX1,
                                   z * noiseFrequencyZ1 + noiseOffsetY1) * noiseAmplitude1 +
                               Mathf.PerlinNoise(x * noiseFrequencyX2 + noiseOffsetX2,
                                   z * noiseFrequencyZ2 + noiseOffsetY2) * noiAmplitude2) * noiseStrength;
                    _vertices[i] = new Vector3(x, y, z);
                    ++i;
                }
            }

            _triangles = TerrainConverter.CreateTriangles(terrainSizeX);

            TerrainGenerated?.Invoke(_vertices, terrainSizeX + 1, terrainSizeY + 1);
            yield break;
        }

        private void Update()
        {
            UpdateMesh();
        }

        private void UpdateMesh()
        {
            mesh.Clear();

            mesh.vertices = _vertices;
            mesh.triangles = _triangles;

            mesh.RecalculateNormals();
        }

        [ContextMenu("SaveTerrainData")]
        private void SaveTerrainData()
        {
            var data = TerrainConverter.ConvertToSerializationFormat(mesh, chunkSize, terrainSizeX);
            Debug.Log("TerrainDataSaved");
            BinarySaveLoadUtility.Save(data, Application.dataPath + "graphData");
        }

        [ContextMenu("LoadTerrainData")]
        private void LoadTerrainData()
        {
            var data = TerrainConverter.ConvertToEngineFormat(
                BinarySaveLoadUtility.Load<TerrainData>(Application.dataPath + "graphData"));
            ConstructChunks(data);
        }

        private List<Vector3> l;
        private void ConstructChunks(Dictionary<Vector3, Mesh> chunksData)
        {
            l = new List<Vector3>();
            foreach (var chunksDataKey in chunksData.Keys)
            {
                l.Add(chunksDataKey);
                var obj = new GameObject(chunksDataKey.ToString());
                obj.AddComponent<MeshFilter>().mesh = chunksData[chunksDataKey];
                obj.AddComponent<MeshRenderer>();
            }
        }

        private void OnDrawGizmos()
        {
            if (l == null)
                return;
            foreach (var VARIABLE in l)
            {
                Gizmos.DrawSphere(VARIABLE, 0.1f);    
            }
            
        }
    }
}