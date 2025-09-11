using System.Collections;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public Vector2Int gridPosition;
    public bool isOccupied = false;

    public SpriteRenderer spriteRenderer;
    public GameObject clearEffectPrefab;

    [Header("Default Appearance")]
    public Sprite defaultSprite;
    public Color defaultColor = Color.white;

    private Vector3 originalScale;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (defaultSprite == null && spriteRenderer != null)
            defaultSprite = spriteRenderer.sprite;
        if (spriteRenderer != null)
            defaultColor = spriteRenderer.color;

        originalScale = transform.localScale;
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

    public void PlayClearAnimation()
    {
        StartCoroutine(ClearAnimRoutine());
    }

    private IEnumerator ClearAnimRoutine()
    {
        Vector3 originalScale = transform.localScale;

    // 1️⃣ Pop anim
    float t = 0f;
    float duration = 0.2f;
    // 3️⃣ Particle
    if (clearEffectPrefab != null)
    {
        GameObject effect = Instantiate(clearEffectPrefab, transform.position, Quaternion.identity);
        Destroy(effect, 1f);
    }
    
    while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, originalScale * 1.2f, t / duration);
            yield return null;
        }

    t = 0f;
    while (t < duration)
    {
        t += Time.deltaTime;
        transform.localScale = Vector3.Lerp(originalScale * 1.2f, originalScale, t / duration);
        yield return null;
    }

    transform.localScale = originalScale;

    // 2️⃣ clear cell
    ResetAppearance();
    SetOccupied(false);

    
    }

}
