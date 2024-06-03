using UnityEngine;

public class PathNode : MonoBehaviour
{
    public int xPos { get; private set; }
    public int yPos { get; private set; }
    public int zPos { get; private set; }
    public Vector2 position;
    public float gValue;
    public float hValue;
    public PathNode parentNode;
    public bool target;
    public float fValue
    {
        get { return gValue + hValue; }
    }

    private SpriteRenderer _rend;
    private Color activeColor = Color.white;
    private Color deactiveColor = Color.green;


    private void Awake()
    {
        _rend = GetComponent<SpriteRenderer>();
    }

    public void Construct(int x, int y)
    {
        xPos = x;
        yPos = y;
        zPos = -(x + y);
    }   

    public void Activate()
    {
        _rend.color = activeColor;
    }

    public void Deactivate()
    {
        if (target) return;
        _rend.color = deactiveColor;
    }
}
