using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Character : MonoBehaviour
{
    public CharacterStats characterStats { get; private set; }

    [SerializeField] private float _moveSpeed;
    [SerializeField] private int _startMovePoints;
    [SerializeField] private int _initiative;

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
    private void Construct(TacticalGrid grid, CombatController controller)
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
            transform.position = _path[i].position;
            yield return new WaitForSeconds(0.5f);
        }
        _isMoving = false;
        transform.position = _grid.GetGridPosition(transform.position);
        _grid.FindReachableNodes(transform.position, _movePoints);
    }
}
