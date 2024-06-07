using UnityEngine;

public class CameraControll : MonoBehaviour
{
    [SerializeField] private Transform _bottomBounds;
    [SerializeField] private Transform _topBounds;
    [SerializeField] private float _sensence;
    private float _horizontalInput;
    private float _verticalInput;
    
    private void Update()
    {
        GetInputs();
        Move();
        CheckBounds();
    }

    private void GetInputs()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");
    }

    private void Move()
    {
        Vector3 pos = transform.position;
        pos.x += _horizontalInput * _sensence * Time.deltaTime;
        pos.y += _verticalInput * _sensence * Time.deltaTime;
        transform.position = pos;
    }

    private void CheckBounds()
    {
        Vector3 pos = transform.position;
        if (pos.x < _bottomBounds.position.x) pos.x = _bottomBounds.position.x;
        if (pos.x > _topBounds.position.x) pos.x = _topBounds.position.x;
        if (pos.y < _bottomBounds.position.y) pos.y = _bottomBounds.position.y;
        if (pos.y > _topBounds.position.y) pos.y = _topBounds.position.y;
        transform.position = pos;
    }
}
