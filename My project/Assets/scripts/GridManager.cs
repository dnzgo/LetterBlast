using System.Collections.Generic;
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

    private int squareSize = 3;

    // List to save full parts
    private List<int> fullRows = new List<int>();
    private List<int> fullCols = new List<int>();
    private List<Vector2Int> fullSquares = new List<Vector2Int>();

    void Awake()
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
                // Position cells relative to GridManager's transform position
                Vector3 localPosition = new Vector3(x * cellSize - offsetX, y * cellSize - offsetY, 0);
                Vector3 worldPosition = transform.position + localPosition;
                
                GameObject newCell = Instantiate(cellPrefab, worldPosition, Quaternion.identity);
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
        // Return world position relative to GridManager's transform position
        float offsetX = (gridWidth * cellSize) / 2f - cellSize / 2f;
        float offsetY = (gridHeigth * cellSize) / 2f - cellSize / 2f;

        Vector3 localPos = new Vector3(x * cellSize - offsetX, y * cellSize - offsetY, 0f);
        return transform.position + localPos;
    }



    // Check Row
    public void CheckFullRows()
    {
        fullRows.Clear();
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
            if (rowFull) fullRows.Add(y);
        }
    }

    // Check Column
    public void CheckFullColumns()
    {
        fullCols.Clear();
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
            if (colFull) fullCols.Add(x);
        }
    }

    // Check Square
    public void CheckFullSquares()
    {
        fullSquares.Clear();
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
                if (squareFull) fullSquares.Add(new Vector2Int(x, y));
            }
        }
    }

    // Clearing
    public void ClearAllDetected()
    {
        HashSet<Vector2Int> cellsToClear = new HashSet<Vector2Int>();
        int clearedStructures = 0; // row + column + square

        // Row
        foreach (int y in fullRows)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                cellsToClear.Add(new Vector2Int(x, y));
            }
            GameManager.Instance.AddScore(gridWidth * 10);
            clearedStructures++;
        }

        // Column
        foreach (int x in fullCols)
        {
            for (int y = 0; y < gridHeigth; y++)
            {
                cellsToClear.Add(new Vector2Int(x, y));
            }
            GameManager.Instance.AddScore(gridHeigth * 10);
            clearedStructures++;
        }

        // square
        foreach (var pos in fullSquares)
        {
            for (int x = pos.x; x < pos.x + squareSize; x++)
            {
                for (int y = pos.y; y < pos.y + squareSize; y++)
                {
                    cellsToClear.Add(new Vector2Int(x, y));
                }
            }
            GameManager.Instance.AddScore(squareSize * squareSize * 5);
            clearedStructures++;
        }

        foreach (var p in cellsToClear)
        {
            gridCells[p.x, p.y].ResetAppearance();
        }

        // combo points
        if (clearedStructures > 0)
        {
            // multi-clear
            if (clearedStructures > 1)
            {
                int multiBonus = (clearedStructures - 1) * GameManager.Instance.multiClearBonusPerStructure;
                GameManager.Instance.AddScore(multiBonus);
                Debug.Log($"Multi-Clear x{clearedStructures} -> + {multiBonus}");
            }

            // Streak
            GameManager.Instance.comboStreak++;
            int streakBonus = GameManager.Instance.comboStreak * GameManager.Instance.streakBonusPerStep;
            GameManager.Instance.AddScore(streakBonus);
            Debug.Log($"Streak x{GameManager.Instance.comboStreak} → +{streakBonus}");
        }
        else
        {
            if (GameManager.Instance.comboStreak != 0)
                Debug.Log("Streak Broken");
            GameManager.Instance.comboStreak = 0;
        }

    }

    public void CheckAndClearAll()
    {
        CheckFullRows();
        CheckFullColumns();
        CheckFullSquares();
        ClearAllDetected();
    }

    public bool CanPlaceLetter(DragDrop letter)
    {

        int gridSize = gridHeigth; //gridCells.GetLength(0)

        Vector2Int[] shape = letter.GetShapeCells();

        // try to place
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                bool canPlaceHere = true;

                foreach (var offset in shape)
                {
                    int checkX = x + offset.x;
                    int checkY = y + offset.y;

                    // if its out of grid 
                    if (checkX < 0 || checkX >= gridSize || checkY < 0 || checkY >= gridSize)
                    {
                        canPlaceHere = false;
                        break;
                    }
                    // does cell full
                    if (gridCells[checkX, checkY].isOccupied)
                    {
                        canPlaceHere = false;
                        break;
                    }

                }

                if (canPlaceHere) return true; // fits at least one place
            }
        }
        return false; // does not fit any place
    }

}
