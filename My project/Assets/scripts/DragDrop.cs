using System.Collections.Generic;
using UnityEngine;

public class DragDrop : MonoBehaviour
{
    public static bool dragEnabled = true;

    private Vector3 startPosition;
    private bool isDragging = false;

    [Header("Grid Settings")]
    public GridManager gridManager;

    public Spawner spawner;

    public int placedMult = 1;
    public float fingerOffset = 3.0f;

    public Color assignedColor;

    private Vector2Int[] cachedShapeOffsets = null;
    private bool offsetsComputed = false;
    void Awake()
    {
        ComputeShapeOffsets();
    }
    void Start()
    {
        if (gridManager == null) gridManager = FindFirstObjectByType<GridManager>();
        if (spawner == null) spawner = FindFirstObjectByType<Spawner>();
    }

    void OnMouseDown()
    {
        if (!dragEnabled) return; // if GameHUD is not active do not make anything
        startPosition = transform.position;
        transform.localScale = Vector3.one; // normalize the size to fit grid
        isDragging = true;
    }

    void OnMouseDrag()
    {
        if (!dragEnabled) return; // if GameHUD is not active do not make anything
        if (isDragging)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePos.x, mousePos.y + fingerOffset, 0); // added ofset manually
        }
    }

    void OnMouseUp()
    {
        if (!dragEnabled) return; // if GameHUD is not active do not make anything
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
        
        // change cell to grid coordinates
        float offsetX = (gridManager.gridWidth * gridManager.cellSize) / 2f - gridManager.cellSize / 2f;
        float offsetY = (gridManager.gridHeigth * gridManager.cellSize) / 2f - gridManager.cellSize / 2f;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform cell = transform.GetChild(i);
            Vector3 localPos = cell.position - gridManager.transform.position;

            int gridX = Mathf.RoundToInt((localPos.x + offsetX) / gridManager.cellSize);
            int gridY = Mathf.RoundToInt((localPos.y + offsetY) / gridManager.cellSize);

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
                Vector3 localPos = cell.position - gridManager.transform.position;
                int gridX = Mathf.RoundToInt((localPos.x + offsetX) / gridManager.cellSize);
                int gridY = Mathf.RoundToInt((localPos.y + offsetY) / gridManager.cellSize);

                Cell gridCell = gridManager.GetCell(gridX, gridY);
                gridCell.SetOccupied(true);

                // copy letterCell renderer to gridCell
                var letterRenderer = cell.GetComponent<SpriteRenderer>();
                gridCell.CopyAppearanceFrom(letterRenderer);
            }
            // increase score
            Vector3 worldPos = transform.position;
            int cellCount = transform.childCount;
            int point = cellCount * placedMult;
            GameManager.Instance.AddScore(point, worldPos, false);
            gridManager.CheckAndClearAll();

            if (spawner != null) spawner.LetterPlaced(gameObject);
            Destroy(gameObject);

            return true;
        }

        return false; // not suitable return to start pos
    }

    public Vector2Int[] GetShapeCells()
    {
        if (!offsetsComputed) ComputeShapeOffsets();
        return cachedShapeOffsets;
    }

    private void ComputeShapeOffsets()
    {
        var list = new List<Vector2Int>();
        foreach (Transform child in transform)
        {
            if (!child.CompareTag("letterCell")) continue;

            int x = Mathf.RoundToInt(child.localPosition.x);
            int y = Mathf.RoundToInt(child.localPosition.y);
            list.Add(new Vector2Int(x, y));
        }
        cachedShapeOffsets = list.ToArray();
        offsetsComputed = true;
    }

}
