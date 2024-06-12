using UnityEngine;

public class Attack
{
    private CharacterStats _stats;
    private Vector2Int _damage;

    public Attack(CharacterStats stats)
    {
        _stats = stats;
        _damage = new Vector2Int(_stats.strenght - 8, _stats.strenght - 5);
    }

    public void AttackTarget(Character target)
    {
        int damage = Random.Range(_damage.x, _damage.y);
        target.TakeDamage(damage);
    }
}
