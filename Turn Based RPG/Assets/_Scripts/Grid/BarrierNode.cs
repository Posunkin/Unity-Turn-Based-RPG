using UnityEngine;
using Zenject;

public class BarrierNode : PathNode
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
        type = NodeType.Barrier;
        activeColor = Color.white;
        deactiveColor = Color.black;
    }
}
