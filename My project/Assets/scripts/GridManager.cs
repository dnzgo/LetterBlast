using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridWidth = 10;
    public int gridHeigth = 10;
    public float cellSize = 1.0f;
    [Header("References")]
    public GameObject cellPrefab;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        float offsetX = (gridWidth * cellSize) / 2f - cellSize / 2f;
        float offsetY = (gridHeigth * cellSize) / 2f - cellSize / 2f;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeigth; y++)
            {
                Vector2 position = new Vector2(x * cellSize - offsetX, y * cellSize - offsetY);
                GameObject newCell = Instantiate(cellPrefab, position, Quaternion.identity);
                newCell.transform.parent = this.transform;

                Cell cell = newCell.GetComponent<Cell>();
                cell.gridPosition = new Vector2Int(x, y);
                cell.SetOccupied(false);
            }
        }
    }
}
