using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Character : MonoBehaviour
{
    public Action OnActionEnd;
    public CharacterStats characterStats { get; private set; }

    [SerializeField] private float _moveSpeed;
    [SerializeField] private int _movePoints;
    [SerializeField] private int _initiative;

    private TacticalGrid _grid;
    private CombatController _combatController;

    private bool _isMoving;
    private bool _selected;
    private List<PathNode> _path = new List<PathNode>();

    private void Start()
    {
        characterStats = new CharacterStats(_movePoints, _initiative);
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
        _grid.FindReachableNodes(transform.position, _movePoints);
        _selected = true;
        while (_selected)
        {
            yield return null;
            if (!_isMoving)
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _grid.DrawPath(transform.position, mousePos);
                if (Input.GetMouseButtonDown(0) && !_isMoving)
                {
                    _path = _grid.GetPath(transform.position, mousePos);
                    if (_path != null) StartCoroutine(MoveRoutine());
                }
            }
        }
    }

    private IEnumerator MoveRoutine()
    {
        _isMoving = true;
        for (int i = 0; i < _path.Count; i++)
        {
            transform.position = _path[i].position;
            yield return new WaitForSeconds(0.5f);
        }
        _isMoving = false;
        transform.position = _grid.GetGridPosition(transform.position);
        _grid.FindReachableNodes(transform.position, _movePoints);
        OnActionEnd?.Invoke();
    }
}
