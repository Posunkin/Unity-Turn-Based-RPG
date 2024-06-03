using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    public List<PathNode> FindPath(Vector2 startPos, Vector2 endPos)
    {
        PathNode startNode = _pathNodes[startPos];
        PathNode endNode = _pathNodes[endPos];

        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();

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

            List<PathNode> neighbours = GetNeighbours(currentNode.position);

            foreach (var neighbour in neighbours)
            {
                if (closedList.Contains(neighbour)) continue;

                float movementCost = currentNode.gValue + CalculateDistance(currentNode, neighbour);
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
            // for (int i = 0; i < neighbours.Count; i++)
            // {
            //     if (closedList.Contains(neighbours[i])) continue;

            //     float movementCost = currentNode.gValue + CalculateDistance(currentNode, neighbours[i]);

            //     if (!openList.Contains(neighbours[i]) || movementCost < neighbours[i].gValue)
            //     {
            //         neighbours[i].gValue = movementCost;
            //         neighbours[i].hValue = CalculateDistance(neighbours[i], endNode);
            //         neighbours[i].parentNode = currentNode;

            //         if (!openList.Contains(neighbours[i]))
            //         {
            //             openList.Add(neighbours[i]);
            //         }
            //     }
            // }
        }

        Debug.Log("Path isn't found!");
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

    public List<PathNode> GetNeighbours(Vector2 position)
    {
        List<PathNode> neighbours = new List<PathNode>();
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

        // Vector2[] directions;

        // if (startY % 2 == 0)
        // {
        //     directions = new Vector2[] {
        //         new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(0, -1),
        //         new Vector2(1, -1), new Vector2(1, 1)
        //     };
        // }
        // else
        // {
        //     directions = new Vector2[] {
        //         new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(0, -1),
        //         new Vector2(-1, -1), new Vector2(-1, 1)
        //     };
        // }

        // foreach (Vector2 dir in directions)
        // {
        //     int newX = startX + (int)dir.x;
        //     int newY = startY + (int)dir.y;

        //     if (newX >= 0 && newX < _width && newY >= 0 && newY < _height)
        //     {
        //         neighbours.Add(_pathNodes[_pathNodesPositions[newX, newY]]);
        //     }
        // }

        // return neighbours;

        if (startX + 1 < _width) neighbours.Add(_pathNodes[_pathNodesPositions[startX + 1, startY]]);
        if (startX - 1 >= 0) neighbours.Add(_pathNodes[_pathNodesPositions[startX - 1, startY]]);
        if (startY + 1 < _height) neighbours.Add(_pathNodes[_pathNodesPositions[startX, startY + 1]]);
        if (startY - 1 >= 0) neighbours.Add(_pathNodes[_pathNodesPositions[startX, startY - 1]]);

        if (startY % 2 == 0)
        {
            if (startX - 1 >= 0 && startY + 1 < _height) neighbours.Add(_pathNodes[_pathNodesPositions[startX - 1, startY + 1]]);
            if (startX - 1 >= 0 && startY - 1 >= 0) neighbours.Add(_pathNodes[_pathNodesPositions[startX - 1, startY - 1]]);
        }
        else
        {
            if (startX + 1 < _width && startY + 1 < _height) neighbours.Add(_pathNodes[_pathNodesPositions[startX + 1, startY + 1]]);
            if (startX + 1 < _width && startY - 1 >= 0) neighbours.Add(_pathNodes[_pathNodesPositions[startX + 1, startY - 1]]);
        }

        return neighbours;
    }

    private float CalculateDistance(PathNode current, PathNode target)
    {
       
        // return (MathF.Abs(current.yPos - target.yPos) + Mathf.Abs(current.xPos - target.yPos) + Mathf.Abs(current.xPos + current.yPos - target.xPos - target.yPos)) / 2;
        return Mathf.Max(Mathf.Abs(current.xPos - target.xPos), Mathf.Max(Mathf.Abs(current.yPos - target.yPos), Mathf.Abs((current.xPos + target.yPos) - (current.xPos + target.yPos))));
    }
}
