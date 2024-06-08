using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public enum CharacterFraction
{
    Ally,
    Enemy
}

public class Character : MonoBehaviour
{
    public CharacterStats characterStats { get; private set; }
    public string characterName { get => _characterName; }
    public Sprite portrait { get => _portrait; }
    public CharacterFraction fraction { get => _fraction; }

    [Header("Combat parameters")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private int _startMovePoints;
    [SerializeField] private int _initiative;
    [SerializeField] private CharacterFraction _fraction;

    [Header("UI Elements"), Space(5)]
    [SerializeField] private string _characterName;
    [SerializeField] private Sprite _portrait;
   

    private TacticalGrid _grid;
    private CombatController _combatController;

    private int _movePoints;
    private bool _isMoving;
    private bool _selected;
    private List<PathNode> _path = new List<PathNode>();

    private void Start()
    {
        characterStats = new CharacterStats(_startMovePoints, _initiative);
        _combatController.AddCharacter(this);
        transform.position = _grid.GetGridPosition(transform.position);
        _isMoving = false;
        _selected = false;
    }

    [Inject]
    private void Dependencies(TacticalGrid grid, CombatController controller)
    {
        _grid = grid;
        _combatController = controller;
    }

    public IEnumerator StartTurn()
    {
        _movePoints = _startMovePoints;
        _grid.FindReachableNodes(transform.position, _movePoints);
        _selected = true;
        while (_selected)
        {
            yield return null;
            if (!_isMoving && _movePoints > 0)
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _grid.DrawPath(transform.position, mousePos);
                if (Input.GetMouseButtonDown(0) && !_isMoving)
                {
                    _path = _grid.GetPath(transform.position, mousePos);
                    if (_path != null) StartCoroutine(MoveRoutine());
                }
            }
            else if (!_isMoving)
            {
                _grid.Clear(transform.position);
            }
        }
    }

    public void EndTurn()
    {
        _selected = false;
    }

    private IEnumerator MoveRoutine()
    {
        _isMoving = true;
        for (int i = 0; i < _path.Count; i++)
        {
            _movePoints -= (int)(3 * _path[i].movePenalty);
            while (Vector2.Distance(_grid.GetGridPosition(transform.position), _path[i].position) > 0.1f)
            {
                transform.position = Vector2.MoveTowards(transform.position, _path[i].position, _moveSpeed * Time.deltaTime);
                yield return null;
            }
            transform.position = _path[i].position;
        }
        _isMoving = false;
        transform.position = _grid.GetGridPosition(transform.position);
        _grid.FindReachableNodes(transform.position, _movePoints);
    }
}
