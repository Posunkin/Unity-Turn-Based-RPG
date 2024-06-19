using System.Collections.Generic;
using UnityEngine;

public class CharacterUI : MonoBehaviour
{
    [SerializeField] private BarUI _healthbar;
    [SerializeField] private BarUI _energybar;
    [SerializeField] private BarUI _movementbar;
    [SerializeField] private ActionPointUI _actionPointPrefab;
    [SerializeField] private Transform _actionPointsContainer;
    private List<ActionPointUI> _actionPoints = new List<ActionPointUI>();
    private Stack<ActionPointUI> _currentPoints = new Stack<ActionPointUI>();

    public void SetUI(int maxHealth, int currentHealth, int energy, int movement, int actionPoints)
    {
        if (_currentPoints != null) _currentPoints.Clear();
        if (_actionPoints != null)
        {
            foreach (var actionPoint in _actionPoints)
            {
                actionPoint.gameObject.SetActive(false);
            }
        }
        _healthbar.InitBar(maxHealth);
        _healthbar.ChangeValue(currentHealth);
        _energybar.InitBar(energy);
        _movementbar.InitBar(movement);
        while (_actionPoints.Count < actionPoints)
        {
            AddActionPoint();
        }
        for (int i = 0; i < actionPoints; i++)
        {
            _currentPoints.Push(_actionPoints[i]);
            _actionPoints[i].Init();
            _actionPoints[i].gameObject.SetActive(true);
        }
    }

    private void AddActionPoint()
    {
        ActionPointUI point = Instantiate(_actionPointPrefab, _actionPointsContainer);
        point.Disable();
        _actionPoints.Add(point);
    }

    public void DisablePoint()
    {
        ActionPointUI point = _currentPoints.Pop();
        point.Disable();
    }

    public void ChangeMovement(int movement)
    {
        _movementbar.ChangeValue(movement);
    }

    public void ChangeHealth(int health)
    {
        _healthbar.ChangeValue(health);
    }

    public void ChangeEnergy(int energy)
    {
        _energybar.ChangeValue(energy);
    }
}
