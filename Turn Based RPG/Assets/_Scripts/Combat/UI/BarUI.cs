using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarUI : MonoBehaviour
{
    [SerializeField] protected Slider _slider;
    [SerializeField] protected TextMeshProUGUI _value;
    [SerializeField] private Image _fill;
    [SerializeField] private Color _fillColor;

    protected int _maxValue;
    protected int _currentValue;

    public void InitBar(int maxValue)
    {
        _fill.color = _fillColor;
        _maxValue = maxValue;
        _currentValue = maxValue;
        _slider.maxValue = _maxValue;
        _slider.value = _currentValue;
        _value.text = $"{_currentValue} / {_maxValue}";
    }

    public void ChangeValue(int value)
    {
        _currentValue = value > 0 ? value : 0;
        _slider.value = _currentValue;
        _value.text = $"{_currentValue} / {_maxValue}";
    }
}
