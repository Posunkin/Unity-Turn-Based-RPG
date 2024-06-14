using UnityEngine;
using UnityEngine.UI;

public class ActionPointUI : MonoBehaviour
{
    [SerializeField] private Image _icon;
    private Color _activeColor = Color.green;
    private Color _unactiveColor = Color.grey;

    public void Init()
    {
        _icon.color = _activeColor;
    }

    public void Disable()
    {
        _icon.color = _unactiveColor;
    }
}
