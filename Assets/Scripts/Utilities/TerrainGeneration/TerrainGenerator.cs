using System;
using System.Collections;
using Autodesk.Fbx;
using UnityEditor.Formats.Fbx.Exporter;
using UnityEngine;
using UnityEngine.Serialization;

namespace TerrainGeneration
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter))]
    public class TerrainGenerator : MonoBehaviour
    {
        [SerializeField] private Mesh mesh;
        [SerializeField] private int xSize;
        [SerializeField] private int zSize;

        [SerializeField] private Vector3[] verticies;
        [SerializeField] private int[] triangles;

        [ContextMenu("InitMesh")]
        private void InitMesh()
        {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
        }

        [ContextMenu("GenerateMapMesh")]
        private void GenerateMesh()
        {
            StartCoroutine(GenerateMeshCoroutine());
            
        }
        private IEnumerator GenerateMeshCoroutine()
        {
            verticies = new Vector3[(xSize + 1) * (zSize + 1)];


            for (int i = 0, z = 0; z <= zSize; z++)
            {
                for (int x = 0; x <= xSize; x++)
                {
                    verticies[i] = new Vector3(x, 0, z);
                    ++i;
                }
            }

            triangles = new int[xSize * zSize * 6];

            int vert = 0;
            int tris = 0;

            for (int z = 0; z < zSize; z++)
            {
                for (int x = 0; x < xSize; x++)
                {
                    triangles[tris + 0] = vert + 0;
                    triangles[tris + 1] = vert + xSize + 1;
                    triangles[tris + 2] = vert + 1;
                    triangles[tris + 3] = vert + 1;
                    triangles[tris + 4] = vert + xSize + 1;
                    triangles[tris + 5] = vert + xSize + 2;

                    vert++;
                    tris += 6;

                    yield return null;
                }

                vert++;
            }
        }

        private void Update()
        {
            UpdateMesh();
        }

        private void UpdateMesh()
        {
            mesh.Clear();

            mesh.vertices = verticies;
            mesh.triangles = triangles;

            mesh.RecalculateNormals();
        }
        
        private void OnDrawGizmos()
        {
            if (verticies == null)
                return;

            for (int i = 0; i < verticies.Length; i++)
            {
                Gizmos.DrawSphere(verticies[i], 0.1f);
            }
        }
        
    }
}