using Unity.VisualScripting;
using UnityEngine;

public class DragDrop : MonoBehaviour
{
    private Vector3 startPosition;
    private bool isDragging = false;

    void OnMouseDown()
    {
        startPosition = transform.position;
        isDragging = true;
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePos.x, mousePos.y, 0);
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
        // todo: to fit grid
        transform.position = startPosition;
    }

}
