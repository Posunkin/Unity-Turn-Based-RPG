using UnityEngine;

public class PathNode : MonoBehaviour
{
    public Vector2 position;
    private SpriteRenderer _rend;
    private Color activeColor = Color.white;
    private Color deactiveColor = Color.green;

    private void Awake()
    {
        _rend = GetComponent<SpriteRenderer>();
    }

    public void Activate()
    {
        _rend.color = activeColor;
    }

    public void Deactivate()
    {
        _rend.color = deactiveColor;
    }
}
