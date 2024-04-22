using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TerrainGeneration;
using UnityEngine;


[RequireComponent(typeof(TerrainGenerator))]
public class GraphConstructor : MonoBehaviour
{
    private Vector3[] _domain;
    private Node[,] _domains2Dim;
    private int _yDomainDimention;
    private int _xDomainDimention;

    private TerrainGenerator _terrainGenerator;

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
    }

    private void ResetNodes()
    {
        for (int y = 0; y < _yDomainDimention; y++)
        {
            for (int x = 0; x < _xDomainDimention; x++)
            {
                _domains2Dim[y, x].Undischarged();
            }
        }
    }

    private void ConvertTo2DimentionalGraph()
    {
        _domains2Dim = new Node[_yDomainDimention, _xDomainDimention];
        for (int i = 0, y = 0; y < _yDomainDimention; y++)
        {
            for (int x = 0; x < _xDomainDimention; x++)
            {
                var node = new Node(_domain[i], new Vector2Int(x, y), _terrainGenerator.Mesh.normals[i]);
                _domains2Dim[y, x] = node;
                ++i;
            }
        }
    }

    [ContextMenu("DDD")]
    private void D()
    {
        var n1 = _domains2Dim[0, 0];
        var n2 = _domains2Dim[0, _domains2Dim.GetLength(1) - 1];
        var n3 = _domains2Dim[_domains2Dim.GetLength(0) - 1, 0];
        

        var rangeX = (n2.Position - n1.Position).magnitude;
        var rangeY = (n3.Position - n1.Position).magnitude;

        
        int countY = (int)(rangeY / maxDeltaY);
        int countX = (int)(rangeX / maxDeltaX);

        
        for (int y = 0; y < countY; y++)
        {
            for (int x = 0; x < countX; x++)
            {
                ResetNodes();
                //var n = FindNearest(posStart.position);
                var n = FindNearest(n1.Position + new Vector3(maxDeltaX * x, maxDeltaY * y));
                DischargeGrid(n, maxDeltaX * x, maxDeltaY * y);
                Debug.Log(maxDeltaX * x + " " + maxDeltaY * y);
            }
        }
        
    }


    [SerializeField] private float minDischargeAngle = 1f;

    [SerializeField] private float maxDeltaX = 20f;
    [SerializeField] private float maxDeltaY = 20f;
    private void DischargeGrid(Node n, float maxX, float maxY)
    {
        if (n.Position.x > maxX || n.Position.z > maxY)
        {
            Debug.Log(n.Position);
            return;
        }
            
        n.Discharged();
        for (int i = 0; i < n.Neighbors.Count; i++)
        {
            if (Vector3.Angle(n.Neighbors[i].Normal, n.Normal) <= minDischargeAngle)
            {
                var nextNeighbor = FindNearNeighbor((n.Neighbors[i].Position - n.Position).normalized * 2f + n.Position,
                    n.Neighbors[i].Neighbors, n);

                if (nextNeighbor == null)
                    continue;
                
                if(n.Neighbors[i].DischargeDone)
                    continue;
                n.Neighbors[i].Deactivate();
                n.Neighbors[i] = nextNeighbor;
              
            }
            
            if(n.Neighbors[i].DischargeDone)
                continue;
            
            DischargeGrid(n.Neighbors[i], maxX, maxY);
        }
        
    }

    private void PrintNeighbors(Node n)
    {
        Debug.Log("parent = " + n.Position);
        foreach (var node in n.Neighbors)
        {
            Debug.Log(node.Position);
        }
    }

    //possible null
    private Node FindNearNeighbor(Vector3 point, List<Node> neighbors, Node notInNeighbors)
    {
        var currentDist = float.MaxValue;
        Node currentNeighbor = null;
        for (int i = 0; i < neighbors.Count; i++)
        {
            var neighbor = neighbors[i];

            if (neighbor.Deactivated)
                continue;

            if(notInNeighbors.Neighbors.Contains(neighbor))
                continue;
            
            if ((neighbor.Position - point).magnitude < currentDist)
            {
                currentDist = (neighbor.Position - point).magnitude;
                currentNeighbor = neighbor;
            }
        }

        return currentNeighbor;
    }

    //public Vector2Int coordStart, coordTarget;
    public Transform posStart, posTarget;

    [ContextMenu("FindPath")]
    private void FindPath()
    {
        var nodeStart = FindNearest(posStart.position);
        var nodeTarget = FindNearest(posTarget.position);
        Debug.Log(nodeStart.Position + " neighbors = " + nodeStart.Neighbors.Count);
        Debug.Log(nodeTarget.Position + " neighbors = " + nodeTarget.Neighbors.Count);

        AStarPathfinder.FindPath(nodeStart, nodeTarget);
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

    private void OnDrawGizmos()
    {
        if (_domains2Dim == null)
            return;

        for (int i = 0, y = 0; y < _yDomainDimention; y++)
        {
            for (int x = 0; x < _xDomainDimention; x++)
            {
                if (_domains2Dim[y, x] == null || _domains2Dim[y, x].Deactivated)
                    continue;


                //Gizmos.DrawRay(_domains2Dim[y, x].Position, _domains2Dim[y, x].Normal);
                ++i;
                if (_domains2Dim[y, x].color != new Color(0, 0, 0, 0))
                    Gizmos.color = _domains2Dim[y, x].color;
                if (_domains2Dim[y, x].Deactivated)
                    continue;
                Gizmos.DrawSphere(_domains2Dim[y, x].Position, 0.1f);
                foreach (var neighbor in _domains2Dim[y, x].Neighbors)
                {
                    if(neighbor.Deactivated)
                        continue;
                    Gizmos.DrawLine(_domains2Dim[y, x].Position, neighbor.Position);
                }
            }
        }
    }
}