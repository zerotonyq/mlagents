using System;
using System.Collections;
using Autodesk.Fbx;
using UnityEditor.Formats.Fbx.Exporter;
using UnityEngine;
using UnityEngine.Serialization;

namespace TerrainGeneration
{
    [RequireComponent(typeof(MeshFilter))]
    public class TerrainGenerator : MonoBehaviour
    {
        [SerializeField] private Mesh mesh;

        public Mesh Mesh => mesh;

        [SerializeField] private int xSize;
        [SerializeField] private int zSize;

        public int XSize => xSize;

        public int ZSize => zSize;

        private Vector3[] _verticies;

        public Vector3[] Verticies => _verticies;

        private int[] _triangles;

        [ContextMenu("InitMesh")]
        private void InitMesh()
        {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
            
        }

        private void Start()
        {
            GenerateMesh();
        }


        [ContextMenu("GenerateMapMesh")]
        private void GenerateMesh()
        {
            StartCoroutine(GenerateMeshCoroutine());
            
        }

        [SerializeField] private float xFrequency1;
        [SerializeField] private float zFrequency1;
        [SerializeField] private float amplitude1;
        [SerializeField] private float xFrequency2;
        [SerializeField] private float zFrequency2;
        [SerializeField] private float amplitude2;
        [SerializeField] private float noiseStrength;
        [SerializeField] private float xOffset1;
        [SerializeField] private float zOffset1;
        [SerializeField] private float xOffset2;
        [SerializeField] private float zOffset2;

        public Action<Vector3[], int, int> TerrainGenerated;
        private IEnumerator GenerateMeshCoroutine()
        {
            _verticies = new Vector3[(xSize + 1) * (zSize + 1)];


            for (int i = 0, z = 0; z <= zSize; z++)
            {
                for (int x = 0; x <= xSize; x++)
                {
                    float y = (Mathf.PerlinNoise(x * xFrequency1 + xOffset1, z * zFrequency1 + zOffset1) * amplitude1 + 
                              Mathf.PerlinNoise(x * xFrequency2 + xOffset2, z * zFrequency2 + zOffset2) * amplitude2) * noiseStrength;
                    _verticies[i] = new Vector3(x, y, z);
                    ++i;
                }
            }

            _triangles = new int[xSize * zSize * 6];

            int vert = 0;
            int tris = 0;

            for (int z = 0; z < zSize; z++)
            {
                for (int x = 0; x < xSize; x++)
                {
                    _triangles[tris + 0] = vert + 0;
                    _triangles[tris + 1] = vert + xSize + 1;
                    _triangles[tris + 2] = vert + 1;
                    _triangles[tris + 3] = vert + 1;
                    _triangles[tris + 4] = vert + xSize + 1;
                    _triangles[tris + 5] = vert + xSize + 2;

                    vert++;
                    tris += 6;

                    yield return null;
                }

                vert++;
            }

            TerrainGenerated?.Invoke(_verticies, xSize + 1, zSize + 1);
        }

        private void Update()
        {
            UpdateMesh();
        }

        private void UpdateMesh()
        {
            mesh.Clear();

            mesh.vertices = _verticies;
            mesh.triangles = _triangles;

            mesh.RecalculateNormals();
        }
        
        
        
    }
}