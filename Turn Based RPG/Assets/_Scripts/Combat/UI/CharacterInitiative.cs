using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInitiative : MonoBehaviour
{
    [SerializeField] private Image _portrait;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private RectTransform _rt;
    [SerializeField] private Outline _outline;

    private Vector2 _turnSize = new Vector2(1f, 1.1f);
    private Vector2 _defSize = new Vector2(0.75f, 0.85f);
    private Color _allyColor = Color.blue;
    private Color _enemyColor = Color.red;
    private Color _portraitColor;

    public void Set(Sprite portrait, string name, CharacterFraction fraction)
    {
        _portrait.sprite = portrait;
        _portraitColor = _portrait.color;
        _name.text = name;
        _outline.enabled = false;
        _outline.effectColor = fraction == CharacterFraction.Ally ? _allyColor : _enemyColor;
    }

    public void TurnStart()
    {
        _rt.localScale = _turnSize;
        _outline.enabled = true;
    }

    public void Highlight(Color color)
    {
        _portrait.color = color;
    }

    public void Unlight()
    {
        _portrait.color = _portraitColor;
    }

    public void TurnEnd()
    {
        _rt.localScale = _defSize;
        _outline.enabled = false;
    }
    
}
