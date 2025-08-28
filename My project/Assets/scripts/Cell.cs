using UnityEngine;

public class Cell : MonoBehaviour
{
    public Vector2Int gridPosition;
    public bool isOccupied = false;

    public void SetOccupied(bool value)
    {
        isOccupied = value;
        GetComponent<SpriteRenderer>().color = value ? Color.red : Color.white;
    }
}
