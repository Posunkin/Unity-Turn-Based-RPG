using System.Collections.Generic;
using UnityEngine;

public class TacticalGrid : MonoBehaviour
{
    [Header("Grid parameters:")]
    [SerializeField] private PathNode _groundHighlightPrefab;
    [SerializeField] private int _width;
    [SerializeField] private int _height;

    private float _offsetX = 1;
    private float _offsetY = 0.375f;

    private Dictionary<Vector2, PathNode> _pathNodes = new();
    private Vector2[,] _pathNodesPositions;
    private List<PathNode> _neighbourNodes = new();
    private List<PathNode> _path = new List<PathNode>();
    private List<Vector2> _reachableNodes = new List<Vector2>();
    private Pathfinding _pathfinding;

    private LineRenderer _line;

    private void Awake()
    {
        _line = GetComponent<LineRenderer>();
        _line.enabled = false;
        InitializeGrid();
        _pathfinding = new Pathfinding(_pathNodesPositions, _pathNodes);
    }

    private void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 pos = GetGridPosition(mousePos);
        if (_reachableNodes.Contains(pos))
        {
            foreach (var node in _pathNodes)
            {
                Vector2 key = node.Key;
                if (_pathNodes[key] == null) continue;
                if (pos == key)
                {
                    _pathNodes[key].Target();
                }
                else
                {
                    _pathNodes[key].Untarget();
                }
            }
        }
        // foreach (var node in _pathNodes)
        // {
        //     Vector2 key = node.Key;
        //     _pathNodes[key].Activate();
        // }
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
                node.Construct(x, y);
                node.position = hexPos;
                _pathNodesPositions[x, y] = hexPos;
                _pathNodes[_pathNodesPositions[x, y]] = node;
                node.Deactivate();
            }
        }
    }

    private Vector2 CalculateHexPosition(int x, int y)
    {
        float xPos = x * _offsetX + (y % 2 == 0 ? 0 : _offsetX * 0.5f);
        float yPos = y * _offsetY;

        return new Vector2(xPos, yPos);
    }

    public void AddNode(PathNode node)
    {
        Vector2 pos = node.position;
        if (_pathNodes.ContainsKey(pos))
        {
            Destroy(_pathNodes[pos].gameObject);
            _pathNodes[pos] = node;
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    if (_pathNodesPositions[x, y] == pos)
                    {
                        node.Construct(x, y);
                    }
                }
            }
            node.Deactivate();
        }
        _pathfinding.Refresh(_pathNodes);
    }

    public Vector2 GetGridPosition(Vector2 worldPosition)
    {
        int y = Mathf.FloorToInt(worldPosition.y / _offsetY);
        int x = Mathf.FloorToInt((worldPosition.x - (y % 2 == 0 ? 0 : _offsetX * 0.5f)) / _offsetX);

        if (x >= 0 && x < _width && y >= 0 && y < _height)
        {
            return _pathNodesPositions[x, y];
        }
        else
        {
            return Vector2.zero;
        }
    }

    public List<PathNode> GetPath(Vector2 startPos, Vector2 endPos)
    {
        Vector2 start = GetGridPosition(startPos);
        Vector2 end = GetGridPosition(endPos);
        PathNode endNode = _pathNodes[end];
        if (_path.Contains(endNode))
        {
            return _path;
        }
        else
        {
            Debug.Log("Position is unreachable");
            return null;
        }
    }

    public void DrawPath(Vector2 startPos, Vector2 endPos)
    {
        Vector2 start = GetGridPosition(startPos);
        Vector2 end = GetGridPosition(endPos);
        if (_reachableNodes.Contains(end))
        {
            _path = _pathfinding.FindPath(start, end);
        }
        if (_path != null)
        {
            _line.positionCount = _path.Count + 1;
            _line.SetPosition(0, start);
            for (int i = 0; i < _path.Count; i++)
            {
                _line.SetPosition(i + 1, _path[i].position);
            }
            _line.enabled = true;
        }
    }

    public void FindReachableNodes(Vector2 startPos, int movePoints)
    {
        foreach (var node in _reachableNodes)
        {
            _pathNodes[node].Deactivate();
        }
        _reachableNodes.Clear();
        Vector2 start = GetGridPosition(startPos);
        List<PathNode> nodes = _pathfinding.FindReachableNodes(start, movePoints);
        foreach (var node in nodes)
        {
            _reachableNodes.Add(node.position);
            node.Activate();
            node.Untarget();
        }
    }

    private void OnDrawGizmos()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Vector2 pos = CalculateHexPosition(x, y);
                Gizmos.DrawCube(pos, Vector3.one / 4);
            }
        }
    }

    private Vector2 GizmosPosition(Vector2 pos)
    {
        _offsetX = 1;
        _offsetY = 0.375f * 1;
        int y = Mathf.FloorToInt(pos.y / _offsetY);
        int x = Mathf.FloorToInt((pos.x - (y % 2 == 0 ? 0 : _offsetX * 0.5f)) / _offsetX);
        return new Vector2(x, y);
    }
}
