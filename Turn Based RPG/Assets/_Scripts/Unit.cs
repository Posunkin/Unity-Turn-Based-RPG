using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Unit : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private int _movePoints;
    private TacticalGrid _grid;
    private bool _isMoving;
    private List<PathNode> _path = new List<PathNode>();

    private void Start()
    {
        transform.position = _grid.GetGridPosition(transform.position);
        _grid.FindReachableNodes(transform.position, _movePoints);
        _isMoving = false;
    }

    [Inject]
    private void Construct(TacticalGrid grid)
    {
        _grid = grid;
    }

    private void Update()
    {
        if (_isMoving) return;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _grid.DrawPath(transform.position, mousePos);
        if (Input.GetMouseButtonDown(0) && !_isMoving)
        {
            _path = _grid.GetPath(transform.position, mousePos);
            if (_path != null) StartCoroutine(MoveRoutine());
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
    }
}
