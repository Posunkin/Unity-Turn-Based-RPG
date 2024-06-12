using UnityEngine;

public class GridObject : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _outline;
    private CharacterFraction _fraction;
    private Color _allyColor = Color.blue;
    private Color _enemyColor = Color.red;

    private void Awake()
    {
        _fraction = GetComponent<Character>().fraction;
        if (_fraction == CharacterFraction.Ally) _outline.color = _allyColor;
        else _outline.color = _enemyColor;
        _outline.enabled = false;
    }

    public void HighlightObject(CharacterFraction fr)
    {
        _outline.color = _fraction == fr ? _allyColor : _enemyColor;
        _outline.enabled = true;
    }

    public void UnlightObject()
    {   
        _outline.enabled = false;
    }
}
