using UnityEngine;

public class Fireball
{
    private TacticalGrid _grid;
    private Character _character;
    private int _distance;
    private int _range;
    private int _damage;
    private bool _isTargeting;

    public Fireball(TacticalGrid grid, int range, int distance, int damage, Character character)
    {
        _grid = grid;
        _range = range;
        _distance = distance * 3;
        _damage = damage;
        _isTargeting = false;
        _character = character;
    }

    public void Target(Vector2 mousePos)
    {
        if (!_isTargeting)
        {
            _grid.Clear();
            _isTargeting = true;
            _grid.GetReachableArea(_character.transform.position, _distance, _character.fraction);
        }
        else
        {
            if (_grid.NodeIsReacheable(mousePos))
            {
                _grid.UnlightSpellArea();
                _grid.HighlightSpellArea(mousePos, _range);
            }
            else
            {
                _grid.UnlightSpellArea();
            }
        }
    }

    public void UnCast()
    {
        _isTargeting = false;
    }
}
