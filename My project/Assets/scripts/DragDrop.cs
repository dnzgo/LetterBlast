using Unity.VisualScripting;
using UnityEngine;

public class DragDrop : MonoBehaviour
{
    private Vector3 startPosition;
    private bool isDragging = false;

    [Header("Grid Settings")]
    public GridManager gridManager;

    public Spawner spawner;

    public int placedMult = 5;

    void Start()
    {
        if (gridManager == null) gridManager = FindFirstObjectByType<GridManager>();
        if (spawner == null) spawner = FindFirstObjectByType<Spawner>();
    }

    void OnMouseDown()
    {
        startPosition = transform.position;
        transform.localScale = Vector3.one; // normalize the size to fit grid
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
        if (gridManager != null)
        {
            // drop to grid and check if placement is valid
            if (TrySnapToGrid())
            {
                return; // succesfully snaped
            }
        }
        // if couldnt snap return to start position
        transform.position = startPosition;
        transform.localScale = Vector3.one * 0.5f;
    }

    private bool TrySnapToGrid()
{
    bool canPlace = true;
    Vector3[] targetPositions = new Vector3[transform.childCount];

    for (int i = 0; i < transform.childCount; i++)
    {
        Transform cell = transform.GetChild(i);

        // change cell to grid coordinates
        float offsetX = (gridManager.gridWidth * gridManager.cellSize) / 2f - gridManager.cellSize / 2f;
        float offsetY = (gridManager.gridHeigth * gridManager.cellSize) / 2f - gridManager.cellSize / 2f;

        int gridX = Mathf.RoundToInt((cell.position.x + offsetX) / gridManager.cellSize);
        int gridY = Mathf.RoundToInt((cell.position.y + offsetY) / gridManager.cellSize);

        //  check grid borders
        if (gridX < 0 || gridX >= gridManager.gridWidth || gridY < 0 || gridY >= gridManager.gridHeigth)
        {
            canPlace = false;
            break;
        }

        Cell targetCell = gridManager.GetCell(gridX, gridY);
        if (targetCell.isOccupied)
        {
            canPlace = false;
            break;
        }

        // save world pos for snap
        targetPositions[i] = gridManager.CellWorldPosition(gridX, gridY);
    }

    if (canPlace)
    {
        // move all child cells to snap pos
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).position = targetPositions[i];
        }

        // mark all cells as occupied
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform cell = transform.GetChild(i);
            float offsetX = (gridManager.gridWidth * gridManager.cellSize) / 2f - gridManager.cellSize / 2f;
            float offsetY = (gridManager.gridHeigth * gridManager.cellSize) / 2f - gridManager.cellSize / 2f;
            int gridX = Mathf.RoundToInt((cell.position.x + offsetX) / gridManager.cellSize);
            int gridY = Mathf.RoundToInt((cell.position.y + offsetY) / gridManager.cellSize);

            gridManager.GetCell(gridX, gridY).SetOccupied(true);
        }
        // increase score
        int cellCount = transform.childCount;
        int point = cellCount * placedMult;
        GameManager.Instance.AddScore(point);

        Destroy(gameObject);

        if (spawner != null) spawner.LetterPlaced(gameObject);

        return true;
    }

    return false; // not suitable return to start pos
}


}
