using System;
using UnityEngine;

public class CharacterHealth
{
    public Action<int> OnHealthChanged;
    public int maxHealth { get => _maxHealth; }
    public int currentHealth { get => _currentHealth; }
    private CharacterStats _stats;
    private int _maxHealth;
    private int _currentHealth;

    public CharacterHealth(CharacterStats stats)
    {
        _stats = stats;
        _maxHealth = 15 + _stats.endurance;
        _currentHealth = _maxHealth;
    }

    public void SubstructHealth(int health)
    {
        Debug.Log("Taking " + health + " damage! My health is " + _currentHealth);
        if (health > 0) _currentHealth -= health;
        OnHealthChanged?.Invoke(_currentHealth);
        if (_currentHealth <= 0)
        {
            Debug.Log("I died!");
        }
    }
}
