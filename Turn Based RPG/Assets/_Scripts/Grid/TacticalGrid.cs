using System.Collections.Generic;
using UnityEngine;

public class TacticalGrid : MonoBehaviour
{
    [Header("Grid parameters:")]
    [SerializeField] private PathNode _groundHighlightPrefab;
    [SerializeField] private int _width;
    [SerializeField] private int _height;

    private float offsetX;
    private float offsetY;
    private float _hexWidth = 1;
    private float _hexHeight = 1;

    private Dictionary<Vector2, PathNode> _pathNodes = new();
    private Vector2[,] _pathNodesPositions;

    private void Awake()
    {
        offsetX = _hexWidth;
        offsetY = 0.375f * _hexHeight;
        InitializeGrid();
    }

    private void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 pos = GetGridPosition(mousePos);
        if (_pathNodes.ContainsKey(pos))
        {
            foreach (var node in _pathNodes)
            {
                Vector2 key = node.Key;
                if (pos == key)
                {
                    _pathNodes[key].Activate();
                }
                else
                {
                    _pathNodes[key].Deactivate();
                }
            }
        }
        else
        {
            Debug.Log("Hex is out of range");
        }
    }

    private void InitializeGrid()
    {
        _pathNodesPositions = new Vector2[_width, _height];
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Vector2 hexPos = CalculateHexPosition(x, y);

                PathNode node = Instantiate(_groundHighlightPrefab, hexPos, Quaternion.Euler(60, 0, 0), this.transform);
                node.position = hexPos;
                _pathNodesPositions[x, y] = hexPos;
                _pathNodes[_pathNodesPositions[x,y]] = node;
                node.Deactivate();
            }
        }
    }

    private Vector2 CalculateHexPosition(int x, int y)
    {
        float xPos = x * offsetX + (y % 2 == 0 ? 0 : offsetX * 0.5f);
        float yPos = y * offsetY;

        return new Vector2(xPos, yPos);
    }

    public Vector2 GetGridPosition(Vector2 worldPosition)
    {
        int y = Mathf.FloorToInt(worldPosition.y / offsetY);
        int x = Mathf.FloorToInt((worldPosition.x - (y % 2 == 0 ? 0 : offsetX * 0.5f)) / offsetX);
       
        if (x >= 0 && x < _width && y >= 0 && y < _height)
        {
            return _pathNodesPositions[x, y];
        }
        else
        {
            Debug.Log("Index is out of range");
            return Vector2.zero;
        }
    }
}
