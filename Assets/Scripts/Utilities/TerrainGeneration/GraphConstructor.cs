using System;
using System.Collections;
using System.Collections.Generic;
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

    private void ConvertTo2DimentionalGraph()
    {
        _domains2Dim = new Node[_yDomainDimention, _xDomainDimention];
        for (int i = 0, y = 0; y < _yDomainDimention; y++)
        {
            for (int x = 0; x < _xDomainDimention; x++)
            {
                var node = new Node(_domain[i]);
                _domains2Dim[y, x] = node;
                ++i;
            }
        }
    }

    public Vector2Int coordStart, coordTarget;

    [ContextMenu("FindPath")]
    private void FindPath()
    {
        AStarPathfinder.FindPath(_domains2Dim[coordStart.y, coordStart.x], _domains2Dim[coordTarget.y, coordTarget.x]);
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
                if(x == 0 && y == 0)
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
        
        for (int y = 0; y < _yDomainDimention; y++)
        {
            for (int x = 0; x < _xDomainDimention; x++)
            {
                if(_domains2Dim[y, x] == null)
                    continue;
                
                Gizmos.color = _domains2Dim[y, x].color;
                Gizmos.DrawSphere(_domains2Dim[y, x].Position, 0.1f);
            }
        }
    }
}