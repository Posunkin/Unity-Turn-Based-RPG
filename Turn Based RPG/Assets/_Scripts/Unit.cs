using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private TacticalGrid _grid;
    [SerializeField] private float _moveSpeed;
    private bool _isMoving;
    private List<PathNode> _path = new List<PathNode>();

    private void Start()
    {
        transform.position = _grid.GetGridPosition(transform.position);
        _isMoving = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !_isMoving)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _path = _grid.FindPath(transform.position, mousePos);
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
    }
}
