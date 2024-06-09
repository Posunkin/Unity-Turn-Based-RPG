using UnityEngine;

public class PathNode : MonoBehaviour
{
    public int xPos { get; protected set; }
    public int yPos { get; protected set; }
    public Vector2 position;
    public NodeType type { get; protected set; }
    public float gValue;
    public float hValue;
    public PathNode parentNode;
    public float fValue
    {
        get { return gValue + hValue; }
    }
    public float movePenalty { get; protected set;}
    public IGridObject gridObject;

    protected SpriteRenderer _rend;
    [SerializeField] protected Color activeColor;
    [SerializeField] protected Color deactiveColor;


    protected void Awake()
    {
        _rend = GetComponent<SpriteRenderer>();
        InitNode();
    }

    protected virtual void InitNode()
    {
        type = NodeType.Path;
        movePenalty = 1f;
    }

    public void Construct(int x, int y)
    {
        xPos = x;
        yPos = y;
    }

    public void Target()
    {
        _rend.color = activeColor;
    }   

    public void Untarget()
    {
        _rend.color = deactiveColor;
    }

    public void Activate()
    {
        _rend.enabled = true;
        _rend.color = deactiveColor;
    }

    public void Deactivate()
    {
        _rend.enabled = false;
    }
}
