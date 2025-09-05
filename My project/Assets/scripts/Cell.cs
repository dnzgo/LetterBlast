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
        /*
        // bigger
        float t = 0;
        while (t < 0.1f)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, originalScale * 1.2f, t / 0.1f);
            yield return null;
        }

        // smaller
        t = 0;
        while (t < 0.1f)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale * 1.2f, originalScale, t / 0.1f);
            yield return null;
        }

        transform.localScale = originalScale;
        */
        Vector3 originalScale = transform.localScale;

    // 1️⃣ Pop animasyonu
    float t = 0f;
    float duration = 0.2f;
    // 3️⃣ Particle patlaması
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

    // Küçültme
    t = 0f;
    while (t < duration)
    {
        t += Time.deltaTime;
        transform.localScale = Vector3.Lerp(originalScale * 1.2f, originalScale, t / duration);
        yield return null;
    }

    transform.localScale = originalScale;

    // 2️⃣ Hücreyi temizle
    ResetAppearance();
    SetOccupied(false);

    
    }

}
