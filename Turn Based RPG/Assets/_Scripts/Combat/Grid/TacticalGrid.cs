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
    private List<PathNode> _path = new List<PathNode>();
    private List<PathNode> _reachableNodes = new List<PathNode>();
    private List<PathNode> _walkableNodes = new List<PathNode>();
    private List<PathNode> _attackArea = new List<PathNode>();
    private List<PathNode> _spellArea = new List<PathNode>();
    private PathNode _highlightNode;
    private Pathfinding _pathfinding;

    private LineRenderer _line;

    private void Awake()
    {
        _line = GetComponent<LineRenderer>();
        _line.enabled = false;
        InitializeGrid();
        _pathfinding = new Pathfinding(_pathNodesPositions, _pathNodes);
    }

    #region Grid nodes
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
    #endregion

    #region Grid positions and calculating methods
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

       public bool NodeIsReacheable(Vector2 pos)
    {
        bool nodeIsReacheable = false;
        Vector2 nodePos = GetGridPosition(pos);
        foreach (var node in _reachableNodes)
        {
            if (node.position == nodePos) 
            {
                nodeIsReacheable = true;
            }
        }

        return nodeIsReacheable;
    }

    public int CalculateDistance(Vector2 a, Vector2 b)
    {
        Vector2 nodeA = GetGridPosition(a);
        Vector2 nodeB = GetGridPosition(b);
        List<PathNode> _distance = _pathfinding.FindPath(nodeA, nodeB);
        if (_distance != null) return _distance.Count;
        else return int.MaxValue;
    }

    public int CalculateDistanceToTarget(Vector2 a, Vector2 b)
    {
        Vector2 nodeA = GetGridPosition(a);
        Vector2 nodeB = GetGridPosition(b);
        List<PathNode> _distance = _pathfinding.FindPathToTarget(nodeA, nodeB);
        if (_distance != null) return _distance.Count;
        else return int.MaxValue;
    }

    public void SetGridObject(Vector2 startPos, Vector2 endPos, GridObject obj)
    {
        Vector2 nodeStartPos = GetGridPosition(startPos);
        Vector2 nodeEndPos = GetGridPosition(endPos);
        _pathNodes[nodeStartPos].gridObject = null;
        _pathNodes[nodeEndPos].gridObject = obj;
    }

    public void SetGridObject(Vector2 pos, GridObject obj)
    {
        Vector2 nodePos = GetGridPosition(pos);
        _pathNodes[nodePos].gridObject = obj;
    }

    public GridObject GetGridObject(Vector2 pos)
    {
        Vector2 nodePos = GetGridPosition(pos);
        if (_pathNodes.ContainsKey(nodePos) && _pathNodes[nodePos].gridObject != null)
        {
            GridObject obj = _pathNodes[nodePos].gridObject;
            return obj;
        }
        else
        {
            return null;
        }
    }
    #endregion

    #region Pathfinding    
    public List<PathNode> GetPath(Vector2 endPos)
    {
        PathNode endNode = _pathNodes[GetGridPosition(endPos)];
        if (_path != null && (_reachableNodes.Contains(endNode) || _walkableNodes.Contains(endNode)))
        {
            return _path;
        }
        else
        {
            Debug.Log("Position is unreachable");
            return null;
        }
    }

    /// <summary>
    /// Visualy highlighting path on the grid
    /// </summary>
    /// <param name="startPos"> Path from </param>
    /// <param name="endPos"> to </param> 
    public void DrawPath(Vector2 startPos, Vector2 endPos)
    {
        Vector2 start = GetGridPosition(startPos);
        Vector2 end = GetGridPosition(endPos);
        PathNode startNode = _pathNodes[start];
        PathNode endNode = _pathNodes[end];
        if (!_reachableNodes.Contains(endNode) && !_walkableNodes.Contains(endNode)) 
        {
            _line.enabled = false;
            if (_highlightNode != null) _highlightNode.Untarget();
            return;
        }
        startNode.Target();
        foreach (var node in _walkableNodes)
        {
            node.Untarget();
        }
        if (endNode.gridObject != null)
        {
            _path = _pathfinding.FindPathToTarget(start, end);
            if (_path != null && _path.Count > 0) _path.RemoveAt(_path.Count - 1);
        }
        else
        {
            _path = _pathfinding.FindPath(start, end);
        }
        
        if (_path != null)
        {
            if (_path.Count > 0) 
            {
                int index = _path.Count - 1;
                _highlightNode = _path[index];
                _highlightNode.Target();
            }
            _line.positionCount = _path.Count + 1;
            _line.SetPosition(0, start);
            for (int i = 0; i < _path.Count; i++)
            {
                _line.SetPosition(i + 1, _path[i].position);
            }
            _line.enabled = true;
        }
    }

    /// <summary>
    /// Unlight all nodes with the character node
    /// </summary>
    public void Clear(Vector2 pos)
    {
        Vector2 gridPos = GetGridPosition(pos);
        _pathNodes[gridPos].Deactivate();
        _line.enabled = false;
        foreach (var node in _reachableNodes)
        {
            node.Deactivate();
        }
        foreach (var node in _walkableNodes)
        {
            node.Deactivate();
        }
    }

    /// <summary>
    /// Unlight all nodes without character node
    /// </summary>
    public void Clear()
    {
        _line.enabled = false;
        foreach (var node in _reachableNodes)
        {
            node.Deactivate();
        }
        foreach (var node in _walkableNodes)
        {
            node.Deactivate();
        }
    }

    /// <summary>
    /// Highlighting all walkable nodes, allies, and targets for attack
    /// </summary>
    public void FindReachableNodes(Vector2 startPos, int movePoints)
    {
        ClearNodes();
        Vector2 start = GetGridPosition(startPos);
        PathNode current = _pathNodes[start];
        current.Activate();
        current.Target();
        List<PathNode> nodes = _pathfinding.FindReachableNodes(start, movePoints);
        foreach (var node in nodes)
        {
            if (node.gridObject != null)
            {
                _reachableNodes.Add(node);
                node.Deactivate();
            }
            else
            {
                _walkableNodes.Add(node);
                node.Activate();
                node.Untarget();
            }
        }
    }

    /// <summary>
    /// Clearing the lists of reachable and walkable nodes
    /// </summary>
    private void ClearNodes()
    {
        foreach (var node in _reachableNodes)
        {
            node.Deactivate();
        }
        foreach (var node in _walkableNodes)
        {
            node.Deactivate();
        }
        foreach (var node in _attackArea)
        {
            node.Deactivate();
        }
        _attackArea.Clear();
        _reachableNodes.Clear();
        _walkableNodes.Clear();
    }

    /// <summary>
    /// For range attack
    /// </summary>
    public void FindTargetNodes(Vector2 pos, int attackRange, CharacterFraction fraction)
    {
        if (_attackArea != null) _attackArea.Clear();
        List<PathNode> nodesInRange = _pathfinding.FindReachableNodes(pos, attackRange);
        foreach (var node in nodesInRange)
        {
            if (node.gridObject != null) 
            {
                _attackArea.Add(node);
                if (node.gridObject.fraction == fraction)
                {
                    node.Activate();
                    node.AllyTarget();
                }
                else
                {
                    node.Activate();
                    node.AttackTarget();
                }
            }
        }
    }
    #endregion

    #region Spell casting highlights
    public void GetReachableArea(Vector2 pos, int distance, CharacterFraction fraction)
    {
        ClearNodes();
        Vector2 start = GetGridPosition(pos);
        PathNode current = _pathNodes[start];
        current.Activate();
        current.Target();
        List<PathNode> nodes = _pathfinding.FindReachableNodes(start, distance);
        foreach (var node in nodes)
        {
            _reachableNodes.Add(node);
            if (node.gridObject != null)
            {
                node.Activate();
                if (node.gridObject.fraction == fraction)
                {
                    node.AllyTarget();
                }
                else
                {
                    node.AttackTarget();
                }
            }
            else
            {
                node.Activate();
                node.Untarget();
            }
        }
    }

    public void HighlightSpellArea(Vector2 pos, int range)
    {
        Vector2 nodePos = GetGridPosition(pos);
        _spellArea = _pathfinding.FindReachableNodes(nodePos, range);
        if (_spellArea != null)
        {
            foreach (var node in _spellArea)
            {
                if (node.gridObject == null) node.Target();
            }
        }
    }

    public void UnlightSpellArea()
    {
        if (_spellArea != null)
        {
            foreach (var node in _spellArea)
            {
                if (node.gridObject == null) node.Untarget();
            }
        }
    }
    #endregion

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
