using Unity.Collections;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridWidth = 10;
    public int gridHeigth = 10;
    public float cellSize = 1.0f;
    public int lineMult = 10;

    [Header("References")]
    public GameObject cellPrefab;

    public Cell[,] gridCells; // fill in GanerateGrid

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        gridCells = new Cell[gridWidth, gridHeigth];

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

                gridCells[x, y] = cell;
            }
        }
    }

    public Cell GetCell(int x, int y)
    {
        return gridCells[x, y];
    }

    public Vector3 CellWorldPosition(int x, int y)
    {
        return new Vector3(x * cellSize - (gridWidth * cellSize) / 2f + cellSize / 2f,
                            y * cellSize - (gridHeigth * cellSize) / 2f + cellSize / 2f,
                            0f);
    }

    public void CheckFullRows()
    {
        for (int y = 0; y < gridHeigth; y++)
        {
            bool rowFull = true;

            for (int x = 0; x < gridWidth; x++)
            {
                if (!gridCells[x, y].isOccupied)
                {
                    rowFull = false;
                    break;
                }
            }

            if (rowFull)
            {
                ClearRow(y);
                GameManager.Instance.AddScore(gridWidth * lineMult); // bonus point for clearing
            }
        }
    }

    private void ClearRow(int y)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            gridCells[x, y].SetOccupied(false);
        }
    }

    public void CheckFullColumns()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            bool colFull = true;

            for (int y = 0; y < gridHeigth; y++)
            {
                if (!gridCells[x, y].isOccupied)
                {
                    colFull = false;
                    break;
                }
            }

            if (colFull)
            {
                ClearColumn(x);
                GameManager.Instance.AddScore(gridHeigth * lineMult); // bonus
            }
        }
    }

    private void ClearColumn(int x)
    {
        for (int y = 0; y < gridHeigth; y++)
        {
            gridCells[x, y].SetOccupied(false);
        }
    }
    
    public void CheckFullSquares(int squareSize = 3)
    {
        for (int x = 0; x <= gridWidth - squareSize; x++)
        {
            for (int y = 0; y <= gridHeigth - squareSize; y++)
            {
                bool squareFull = true;

                for (int i = 0; i < squareSize; i++)
                {
                    for (int j = 0; j < squareSize; j++)
                    {
                        if (!gridCells[x + i, y + j].isOccupied)
                        {
                            squareFull = false;
                            break;
                        }
                    }
                    if (!squareFull) break;
                }

                if (squareFull)
                {
                    ClearSquare(x, y, squareSize);
                    GameManager.Instance.AddScore(squareSize * squareSize * 15); // bonus
                }
            }
        }
    }

    private void ClearSquare(int startX, int startY, int size)
    {
        for (int x = startX; x < startX + size; x++)
        {
            for (int y = startY; y < startY + size; y++)
            {
                gridCells[x, y].SetOccupied(false);
            }
        }
    }



}
