using UnityEngine;
using Zenject;

public class ObstacleNode : PathNode
{
    private TacticalGrid _grid;

    [Inject]
    private void Construct(TacticalGrid grid)
    {
        _grid = grid;
    }

    private void Start()
    {
        transform.position = _grid.GetGridPosition(transform.position);
        position = transform.position;
        _grid.AddNode(this);
    }

    protected override void InitNode()
    {
        type = NodeType.Obstacle;
        movePenalty = 1.5f;
    }
}
