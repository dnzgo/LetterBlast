using UnityEngine;

public class Cell : MonoBehaviour
{
    public Vector2Int gridPosition;
    public bool isOccupied = false;

    public SpriteRenderer spriteRenderer;

    [Header("Default Appearance")]
    public Sprite defaultSprite;
    public Color defaultColor = Color.white;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (defaultSprite == null && spriteRenderer != null)
            defaultSprite = spriteRenderer.sprite;
        if (spriteRenderer != null)
            defaultColor = spriteRenderer.color;
    }
    public void SetOccupied(bool value)
    {
        isOccupied = value;
    }

    public void CopyAppearanceFrom(SpriteRenderer source)
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (source != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = source.sprite;
            spriteRenderer.color = source.color;
        }
    }

    public void ResetAppearance()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = defaultSprite;
            spriteRenderer.color = defaultColor;
        }
        isOccupied = false;
    }

}
