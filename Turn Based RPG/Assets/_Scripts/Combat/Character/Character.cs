using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public enum CharacterFraction
{
    Ally,
    Enemy
}

public class Character : MonoBehaviour, IDamageable
{
    public CharacterStats characterStats { get; private set; }
    public string characterName { get => _characterName; }
    public Sprite portrait { get => _portrait; }
    public CharacterFraction fraction { get => _fraction; }
    public CharacterHealth health { get => _health; }

    [Header("Combat parameters")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private int _strenght;
    [SerializeField] private int _endurance;
    [SerializeField] private int _startMovePoints;
    [SerializeField] private int _startActionPoints;
    [SerializeField] private int _initiative;
    [SerializeField] private CharacterFraction _fraction;
    [SerializeField] private int _attackRange;
    private int _actionPoints;
    private int _energy = 10;
    private CharacterHealth _health;
    private Attack _attack;

    [Header("UI Elements"), Space(5)]
    [SerializeField] private string _characterName;
    [SerializeField] private Sprite _portrait;
   

    private TacticalGrid _grid;
    private CombatController _combatController;
    private GridObject _gridObject;
    private CharacterUI _characterUI;

    private PlayerInput _input;
    private int _movePoints;
    private Coroutine _actionRoutine;
    private bool _isDoingAction;
    private bool _selected;
    private List<PathNode> _path = new List<PathNode>();
    private GridObject _lastObj = null;
    private GridObject _selectedObj = null;

    private void Awake()
    {
        _gridObject = GetComponent<GridObject>();
        characterStats = new CharacterStats(_strenght, _endurance, _startMovePoints, _initiative);
        _health = new CharacterHealth(characterStats);
        _attack = new Attack(characterStats);
    }

    private void Start()
    {
        _combatController.AddCharacter(this);
        transform.position = _grid.GetGridPosition(transform.position);
        _grid.SetGridObject(transform.position, _gridObject);
        _isDoingAction = false;
        _selected = false;
    }

    [Inject]
    private void Dependencies(TacticalGrid grid, CombatController controller, PlayerInput input)
    {
        _grid = grid;
        _combatController = controller;
        _input = input;
    }

    public IEnumerator StartTurn(CharacterUI UI)
    {
        _characterUI = UI;
        Initialize(_characterUI);
        _grid.FindReachableNodes(transform.position, _movePoints);
        _grid.FindTargetNodes(transform.position, _attackRange, fraction);
        _selected = true;
        while (_selected)
        {
            yield return null;
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Movement(mousePos);
            ChooseTarget(mousePos);
            AttackTarget();
        }
    }

    private void Initialize(CharacterUI UI)
    {
        _movePoints = _startMovePoints;
        _actionPoints = _startActionPoints;
        _characterUI = UI;
        _characterUI.SetUI(_health.maxHealth, _health.currentHealth, _energy, _movePoints, _actionPoints);
    }

    #region Actions
    private void Movement(Vector2 mousePos)
    {
        if (!_isDoingAction && _movePoints > 0)
        {
            _grid.DrawPath(transform.position, mousePos);
            if (_input.Combat.Select.ReadValue<float>() > 0.5f)
            {
                _path = _grid.GetPath(mousePos);
                if (_path != null)
                {
                    if (_selectedObj == null || _grid.CalculateDistanceToTarget(transform.position, _selectedObj.transform.position) > _attackRange)
                    _actionRoutine = StartCoroutine(MoveRoutine());
                } 
            }
        }
        else if (!_isDoingAction)
        {
            _grid.Clear(transform.position);
            _grid.FindTargetNodes(transform.position, _attackRange, fraction);
        }
    }

    private IEnumerator MoveRoutine()
    {
        _isDoingAction = true;
        Vector2 startPos = transform.position;
        for (int i = 0; i < _path.Count; i++)
        {
            _movePoints -= (int) _path[i].movePenalty;
            while (Vector2.Distance(transform.position, _path[i].position) > 0.01f)
            {
                transform.position = Vector2.MoveTowards(transform.position, _path[i].position, _moveSpeed * Time.deltaTime);
                yield return null;
            }
            transform.position = _path[i].position;
            _characterUI.ChangeMovement(_movePoints);
        }
        _isDoingAction = false;
        transform.position = _grid.GetGridPosition(transform.position);
        Vector2 endPos = transform.position;
        _grid.SetGridObject(startPos, endPos, _gridObject);
        _grid.FindReachableNodes(transform.position, _movePoints);
        _grid.FindTargetNodes(transform.position, _attackRange, fraction);
        _path.Clear();
    }

    private void ChooseTarget(Vector3 mousePos)
    {
        _selectedObj = _grid.GetGridObject(mousePos);
        if (_selectedObj != null && _selectedObj != _gridObject) 
        {
            if (_selectedObj != _lastObj)
            {
                if (_lastObj != null) _lastObj.UnlightObject();
                _lastObj = _selectedObj;
            }
            _selectedObj.HighlightObject(_fraction);
        }
        else
        {
            if (_lastObj != null) _lastObj.UnlightObject();
            _lastObj = null;
        }
    }

    private void AttackTarget()
    {
        if (_selectedObj == null || _actionPoints == 0) return;
        if (_grid.CalculateDistanceToTarget(transform.position, _selectedObj.transform.position) <= _attackRange
        && _input.Combat.Select.triggered && !_isDoingAction)
        {
            _isDoingAction = true;
            Character ch = _selectedObj.GetComponent<Character>();
            Debug.Log(ch);
            _attack.AttackTarget(ch);
            _actionPoints--;
            _characterUI.DisablePoint();
            _actionRoutine = StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        yield return null;
        _isDoingAction = false;
    }
    #endregion
    
    public void EndTurn()
    {
        _selected = false;
        _grid.Clear(transform.position);
    }

    public void TakeDamage(int damage)
    {
        _health.SubstructHealth(damage);
    }
}
