using UnityEngine;

public enum NodeType
{
    Path,
    Obstacle,
    Barrier
}

public class PathNode : MonoBehaviour
{
    public int xPos { get; protected set; }
    public int yPos { get; protected set; }
    public Vector2 position;
    public NodeType type { get; protected set; }
    public float gValue;
    public float hValue;
    public PathNode parentNode;
    public bool target;
    public float fValue
    {
        get { return gValue + hValue; }
    }
    public float movePenalty;

    protected SpriteRenderer _rend;
    protected Color activeColor;
    protected Color deactiveColor;


    protected void Awake()
    {
        _rend = GetComponent<SpriteRenderer>();
        InitNode();
    }

    protected virtual void InitNode()
    {
        type = NodeType.Path;
        movePenalty = 1f;
        activeColor = Color.white;
        deactiveColor = Color.green;
    }

    public void Construct(int x, int y)
    {
        xPos = x;
        yPos = y;
    }   

    public virtual void Activate()
    {
        if (type == NodeType.Barrier) return;
        _rend.color = activeColor;
    }

    public virtual void Deactivate()
    {
        if (target) return;
        _rend.color = deactiveColor;
    }
}
