using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _value;
    private int _maxHealth;
    private int _currentHealth;
    private CharacterHealth _health;

    public void Init(CharacterHealth health, string name)
    {
        _health = health;
        _maxHealth = _health.maxHealth;
        _currentHealth = _maxHealth;
        _slider.maxValue = _maxHealth;
        _slider.value = _maxHealth;
        _name.text = name;
        _value.text = $"{_currentHealth} / {_maxHealth}";
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

    public void ChangeValue(int health)
    {
        if (health > 0)
        {
            _currentHealth = health;
            _slider.value = health;
            _value.text = $"{_currentHealth} / {_maxHealth}";
        } 
        else
        {
            _slider.value = 0;
            _value.text = $"0 / {_maxHealth}";
        }
    }
}
