using UnityEngine;

public class GridObject : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _outline;
    private Healthbar _healthbar;
    private CharacterInitiative _init;
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

    public void Construct(CharacterInitiative init, Healthbar healthbar)
    {
        _init = init;
        _healthbar = healthbar;
    }

    public void HighlightObject(CharacterFraction fr)
    {
        _outline.color = _fraction == fr ? _allyColor : _enemyColor;
        _outline.enabled = true;
        _init.Highlight(_outline.color);
        _healthbar.Highlight();
    }

    public void UnlightObject()
    {   
        _outline.enabled = false;
        _healthbar.Unlight();
        _init.Unlight();
    }
}
