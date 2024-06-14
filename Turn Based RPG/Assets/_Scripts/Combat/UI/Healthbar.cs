using TMPro;
using UnityEngine;

public class Healthbar : BarUI
{
    [SerializeField] private TextMeshProUGUI _name;
    private CharacterHealth _health;

    public void Init(CharacterHealth health, string name)
    {
        _health = health;
        _maxValue = _health.maxHealth;
        _currentValue = _maxValue;
        _slider.maxValue = _maxValue;
        _slider.value = _maxValue;
        _name.text = name;
        _value.text = $"{_currentValue} / {_maxValue}";
        gameObject.SetActive(false);
        _health.OnHealthChanged += ChangeValue;
    }

    private void OnDestroy()
    {
        _health.OnHealthChanged -= ChangeValue;
    }

    public void Highlight()
    {
        gameObject.SetActive(true);
    }

    public void Unlight()
    {
        gameObject.SetActive(false);
    }
}
