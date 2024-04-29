using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TerrainGeneration;
using TerrainGeneration.Data;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities.GraphGenerator.Converter;
using Utilities.SaveLoad;


[RequireComponent(typeof(TerrainGenerator))]
public class GraphGenerator : MonoBehaviour
{
    private Vector3[] _domain;
    private Node[,] _domains2Dim;
    private int _yDomainDimention;
    private int _xDomainDimention;

    private TerrainGenerator _terrainGenerator;

    [SerializeField] private float minDischargeAngle = 1f;

    [SerializeField] private float maxGraphChunkLengthX = 20f;
    [SerializeField] private float maxGraphChunkLengthY = 20f;
    [SerializeField] private float maxAngleToAssignNeighbor = 45f;

    public Transform posStart, posTarget;

    private void Awake()
    {
        _terrainGenerator = GetComponent<TerrainGenerator>();
        _terrainGenerator.TerrainGenerated += (v, x, y) =>
        {
            _domain = v;
            _xDomainDimention = x;
            _yDomainDimention = y;
        };
    }

    [ContextMenu("ConstructGraph")]
    private void ConstructGraph()
    {
        ConvertTo2DimentionalGraph();

        for (int y = 0; y < _yDomainDimention; y++)
        {
            for (int x = 0; x < _xDomainDimention; x++)
            {
                AssignNeighborsForNode(new Vector2Int(x, y));
            }
        }
        Debug.Log("Graph constructed");
    }

    [ContextMenu("DischargeGraph")]
    private void DischargeGraph()
    {
        var n1 = _domains2Dim[0, 0];
        var n2 = _domains2Dim[0, _domains2Dim.GetLength(1) - 1];
        var n3 = _domains2Dim[_domains2Dim.GetLength(0) - 1, 0];


        var rangeX = (n2.Position - n1.Position).magnitude;
        var rangeY = (n3.Position - n1.Position).magnitude;


        int countY = (int)(rangeY / maxGraphChunkLengthY) + 2;
        int countX = (int)(rangeX / maxGraphChunkLengthX) + 2;


        StartCoroutine(DischargeCoroutine(n1, countY, countX));
    }

    private IEnumerator DischargeCoroutine(Node n1, int countY, int countX)
    {
        ResetNodes();

        for (int y = 0; y <= countY; y++)
        {
            for (int x = 0; x <= countX; x++)
            {
                var n = FindNearest(n1.Position +
                                    new Vector3(maxGraphChunkLengthX * (x + 1) / 2, 0,
                                        maxGraphChunkLengthY * (y + 1) / 2));

                DischargeGrid(n, maxGraphChunkLengthX * x, maxGraphChunkLengthY * y);

                yield return new WaitForSeconds(1f);
            }
        }

#if UNITY_EDITOR
        Debug.Log("END");
#endif

        yield return null;
    }

    [ContextMenu("ConvertAndSaveGraph")]
    private void ConvertAndSaveGraph()
    {
        BinarySaveLoadUtility.Save(
            GraphConverter.ConvertToSerializationFormat(_domains2Dim, _xDomainDimention, _yDomainDimention),
            Application.dataPath + "graphData");
    }

    public static List<Node> LoadAndConvertGraph(string path)
    {
        var graphData = BinarySaveLoadUtility.Load<GraphData>(path);

        return GraphConverter.ConvertToEngineFormat(graphData);
    }

    private void ResetNodes()
    {
        for (int y = 0; y < _yDomainDimention; y++)
        {
            for (int x = 0; x < _xDomainDimention; x++)
            {
                _domains2Dim[y, x].SetUndischarged();
            }
        }
    }

    private void ConvertTo2DimentionalGraph()
    {
        _domains2Dim = new Node[_yDomainDimention, _xDomainDimention];
        var mesh = _terrainGenerator.Mesh;
        for (int i = 0, y = 0; y < _yDomainDimention; y++)
        {
            for (int x = 0; x < _xDomainDimention; x++)
            {
                var node = new Node(_domain[i], mesh.normals[i]);
                _domains2Dim[y, x] = node;
                ++i;
            }
        }
    }


    private void DischargeGrid(Node n, float maxX, float maxY)
    {
        n.SetDischarged();

        if (n.Position.x > maxX || n.Position.z > maxY)
            return;

        for (int i = 0; i < n.Neighbors.Count; i++)
        {
            if (Vector3.Angle(n.Neighbors[i].Normal, n.Normal) <= minDischargeAngle)
            {
                var nextNeighbor = FindNearNeighbor(n.Position,
                    (n.Neighbors[i].Position - n.Position).normalized * 100f + n.Position,
                    n.Neighbors[i].Neighbors, n);

                if (nextNeighbor == null)
                    continue;


                if (n.Neighbors[i].Discharged)
                    continue;

                n.Neighbors[i].Deactivate();
                n.Neighbors[i] = nextNeighbor;
            }

            if (n.Neighbors[i].Discharged)
                continue;

            DischargeGrid(n.Neighbors[i], maxX, maxY);
        }
    }


    private Node FindNearNeighbor(Vector3 start, Vector3 dir, List<Node> neighbors, Node notInNeighbors)
    {
        var currentDist = float.MaxValue;
        Node currentNeighbor = null;

        for (int i = 0; i < neighbors.Count; i++)
        {
            var neighbor = neighbors[i];

            if (neighbor.Deactivated)
                continue;

            if (notInNeighbors.Neighbors.Contains(neighbor))
                continue;

            var dirN = neighbor.Position - start;

            if (Vector3.Angle(dir, dirN) > maxAngleToAssignNeighbor)
                continue;

            if ((neighbor.Position - dir).magnitude < currentDist)
            {
                currentDist = (neighbor.Position - dir).magnitude;
                currentNeighbor = neighbor;
            }
        }

        return currentNeighbor;
    }

    private Node FindNearest(Vector3 position)
    {
        float minDist = float.MaxValue;
        Node currentNode = null;
        for (int y = 0; y < _yDomainDimention; y++)
        {
            for (int x = 0; x < _xDomainDimention; x++)
            {
                if (_domains2Dim[y, x] != null && (_domains2Dim[y, x].Position - position).magnitude < minDist)
                {
                    if (_domains2Dim[y, x].Deactivated)
                        continue;
                    currentNode = _domains2Dim[y, x];
                    minDist = (_domains2Dim[y, x].Position - position).magnitude;
                }
            }
        }

        return currentNode;
    }

    private void AssignNeighborsForNode(Vector2Int domainCoord)
    {
        var currentCenter = _domains2Dim[domainCoord.y, domainCoord.x];

        var xDimSize = _domains2Dim.GetLength(1);
        var yDimSize = _domains2Dim.GetLength(0);
        for (int y = -1; y <= 1; ++y)
        {
            for (int x = -1; x <= 1; ++x)
            {
                if (x == 0 && y == 0)
                    continue;

                if (!ValidateCoordinates(domainCoord + new Vector2Int(x, y), xDimSize, yDimSize))
                    continue;

                currentCenter.Neighbors.Add(_domains2Dim[domainCoord.y + y, domainCoord.x + x]);
            }
        }
    }

    private bool ValidateCoordinates(Vector2Int v, int xDimSize, int yDimSize)
    {
        return v.x >= 0 && v.y >= 0 && v.y < yDimSize && v.x < xDimSize;
    }

    [ContextMenu("EnableGizmos")]
    public void ToggleGizmo()
    {
        gizmo = !gizmo;
    }

    public bool gizmo = false;
    private void OnDrawGizmos()
    {
        if(!gizmo)
            return;
        
        if (_domains2Dim == null)
            return;

        for (int i = 0, y = 0; y < _yDomainDimention; y++)
        {
            for (int x = 0; x < _xDomainDimention; x++)
            {
                if (_domains2Dim[y, x] == null || _domains2Dim[y, x].Deactivated)
                    continue;


                ++i;

                if (_domains2Dim[y, x].Deactivated)
                    continue;

                Gizmos.DrawSphere(_domains2Dim[y, x].Position, 0.1f);

                foreach (var neighbor in _domains2Dim[y, x].Neighbors)
                {
                    if (neighbor.Deactivated)
                        continue;

                    Gizmos.DrawLine(_domains2Dim[y, x].Position, neighbor.Position);
                }
            }
        }
    }
}