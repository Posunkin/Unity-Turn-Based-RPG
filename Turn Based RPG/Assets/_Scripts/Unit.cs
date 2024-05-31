using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private TacticalGrid _grid;

    private void Start()
    {
        transform.position = _grid.GetGridPosition(transform.position);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 pos = _grid.GetGridPosition(mousePos);
            transform.position = pos;
        }
    }
}
