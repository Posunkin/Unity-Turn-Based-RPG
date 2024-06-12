using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    private Vector2[,] _pathNodesPositions;
    private Dictionary<Vector2, PathNode> _pathNodes;
    private int _width, _height;

    public Pathfinding(Vector2[,] pathNodesPositions, Dictionary<Vector2, PathNode> pathNodes)
    {
        _pathNodesPositions = pathNodesPositions;
        _width = _pathNodesPositions.GetLength(0);
        _height = _pathNodesPositions.GetLength(1);
        _pathNodes = pathNodes;
    }

    public void Refresh(Dictionary<Vector2, PathNode> pathNodes)
    {
        _pathNodes = pathNodes;
    }

    public List<PathNode> FindPath(Vector2 startPos, Vector2 endPos)
    {
        PathNode startNode = _pathNodes[startPos];
        PathNode endNode = _pathNodes[endPos];

        List<PathNode> openList = new List<PathNode>();
        HashSet<PathNode> closedList = new HashSet<PathNode>();

        startNode.gValue = 0;
        startNode.hValue = CalculateDistance(startNode, endNode);

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            PathNode currentNode = openList[0];

            for (int i = 0; i < openList.Count; i++)
            {
                if (currentNode.fValue > openList[i].fValue || (currentNode.fValue == openList[i].fValue && currentNode.hValue > openList[i].hValue))
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode == endNode)
            {
                return RetracePath(startNode, endNode);
            }

            List<PathNode> neighbours = GetNeighbours(currentNode);

            foreach (var neighbour in neighbours)
            {
                if (closedList.Contains(neighbour) || neighbour.gridObject != null) continue;

                float movementCost = currentNode.gValue + CalculateDistance(currentNode, neighbour) + neighbour.movePenalty;
                if (movementCost < neighbour.gValue || !openList.Contains(neighbour))
                {
                    neighbour.gValue = movementCost;
                    neighbour.hValue = CalculateDistance(neighbour, endNode);
                    neighbour.parentNode = currentNode;

                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                }
            }
        }
        return null;
    }

    public List<PathNode> FindPathToTarget(Vector2 startPos, Vector2 endPos)
    {
        PathNode startNode = _pathNodes[startPos];
        PathNode endNode = _pathNodes[endPos];

        List<PathNode> openList = new List<PathNode>();
        HashSet<PathNode> closedList = new HashSet<PathNode>();

        startNode.gValue = 0;
        startNode.hValue = CalculateDistance(startNode, endNode);

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            PathNode currentNode = openList[0];

            for (int i = 0; i < openList.Count; i++)
            {
                if (currentNode.fValue > openList[i].fValue || (currentNode.fValue == openList[i].fValue && currentNode.hValue > openList[i].hValue))
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode == endNode)
            {
                return RetracePath(startNode, endNode);
            }

            List<PathNode> neighbours = GetNeighbours(currentNode);

            foreach (var neighbour in neighbours)
            {
                if (closedList.Contains(neighbour)) continue;
                if (neighbour.gridObject != null && neighbour != endNode) continue;

                float movementCost = currentNode.gValue + CalculateDistance(currentNode, neighbour) + neighbour.movePenalty;
                if (movementCost < neighbour.gValue || !openList.Contains(neighbour))
                {
                    neighbour.gValue = movementCost;
                    neighbour.hValue = CalculateDistance(neighbour, endNode);
                    neighbour.parentNode = currentNode;

                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                }
            }
        }
        return null;
    }

    private List<PathNode> RetracePath(PathNode startNode, PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();

        PathNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parentNode;
        }
        path.Reverse();

        return path;
    }

    public List<PathNode> GetNeighbours(PathNode node)
    {
        List<PathNode> neighbours = new List<PathNode>();
        Vector2 position = node.position;
        int startX = 0;
        int startY = 0;

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if (_pathNodesPositions[x, y] == position)
                {
                    startX = x;
                    startY = y;
                    break;
                }
            }
        }

        Vector2[] directions;

        if (startY % 2 != 0)
        {
            directions = new Vector2[] {
                new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(0, -1),
                new Vector2(1, -1), new Vector2(1, 1)
            };
        }
        else
        {
            directions = new Vector2[] {
                new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(0, -1),
                new Vector2(-1, -1), new Vector2(-1, 1)
            };
        }

        foreach (Vector2 dir in directions)
        {
            int newX = startX + (int)dir.x;
            int newY = startY + (int)dir.y;

            if (newX >= 0 && newX < _width && newY >= 0 && newY < _height)
            {
                if (_pathNodes[_pathNodesPositions[newX, newY]].type != NodeType.Barrier)
                {
                    neighbours.Add(_pathNodes[_pathNodesPositions[newX, newY]]);
                }
            }
        }

        return neighbours;
    }

    private float CalculateDistance(PathNode current, PathNode target)
    {
        return Mathf.Max(Mathf.Abs(current.xPos - target.xPos), 
        Mathf.Max(Mathf.Abs(current.yPos - target.yPos), 
        Mathf.Abs((current.xPos + target.yPos) - (current.xPos + target.yPos))));
    }

    public List<PathNode> FindReachableNodes(Vector2 startPos, int movePoints)
    {
        if (!_pathNodes.ContainsKey(startPos))
        {
            Debug.LogError("Start position is wrong!");
            return null;
        }

        PathNode startNode = _pathNodes[startPos];
        List<PathNode> reachableNodes = new List<PathNode>();
        Queue<PathNode> openQueue = new Queue<PathNode>();
        Dictionary<PathNode, float> visitedNodes = new Dictionary<PathNode, float>();

        openQueue.Enqueue(startNode);
        visitedNodes[startNode] = 0;

        while (openQueue.Count > 0)
        {
            PathNode currentNode = openQueue.Dequeue();
            float currentCost = visitedNodes[currentNode];

            if (currentCost <= movePoints)
            {
                reachableNodes.Add(currentNode);

                List<PathNode> neighbours = GetNeighbours(currentNode);
                foreach (var neighbour in neighbours)
                {
                    float movementCost = neighbour.movePenalty > 0 ? (1 + neighbour.movePenalty) * 1.5f : 1 + neighbour.movePenalty;
                    float newCost = currentCost + movementCost;

                    if (!visitedNodes.ContainsKey(neighbour) || newCost < visitedNodes[neighbour])
                    {
                        visitedNodes[neighbour] = newCost;
                        openQueue.Enqueue(neighbour);
                    }
                }
            }
        }

        return reachableNodes;
    }
}
