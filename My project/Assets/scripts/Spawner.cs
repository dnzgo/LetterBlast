using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] letterPrefabs;
    public GameObject[] offerPrefabs;
    public float yPos = -4f;
    public float xSpacing = 2f;

    private List<GameObject> activeLetters = new List<GameObject>();

    public Color[] letterColors = new Color[6]
    {
        new Color(40f, 160f, 161f), // Light Sea Green
        new Color(240f, 211f, 186f),   // Dutch White
        new Color(244f, 130f, 115f),     // Salmon
        new Color(227f, 104f, 87f),    // Terra Cotta
        new Color(246f, 222f, 126f),    // Jasmine
        new Color(67f, 146f, 192f)    // Cyan-Blue Azure
    };
    public void SpawnBatch()
    {
        activeLetters.Clear();

        for (int i = -1; i <= 1; i++)
        {
            int index = Random.Range(0, letterPrefabs.Length);
            Vector3 spawnPos = new Vector3(i * xSpacing, yPos, 0);
            GameObject letter = Instantiate(letterPrefabs[index], spawnPos, Quaternion.identity);
            letter.transform.localScale = Vector3.one * 0.5f;  // preview

            DragDrop dragDrop = letter.GetComponent<DragDrop>();
            if (dragDrop != null) dragDrop.gridManager = FindFirstObjectByType<GridManager>();

            dragDrop.spawner = this;

            AssignRandomColor(letter);

            activeLetters.Add(letter);
        }

        if (activeLetters.Count != 0)
        {
            GameManager.Instance.CheckGameOver(activeLetters);
        }


    }
    public void SpawnRewardLetters()
    {
        foreach (var letter in activeLetters)
        {
            if (letter != null)
                Destroy(letter.gameObject);
        }
        activeLetters.Clear();
        for (int i = -1; i <= 1; i++)
        {
            int index = Random.Range(0, offerPrefabs.Length);
            Vector3 spawnPos = new Vector3(i * xSpacing, yPos, 0);
            GameObject letter = Instantiate(offerPrefabs[index], spawnPos, Quaternion.identity);
            letter.transform.localScale = Vector3.one * 0.5f;  // preview

            DragDrop dragDrop = letter.GetComponent<DragDrop>();
            if (dragDrop != null) dragDrop.gridManager = FindFirstObjectByType<GridManager>();

            dragDrop.spawner = this;

            AssignRandomColor(letter);

            activeLetters.Add(letter);
        }

        if (activeLetters.Count != 0)
        {
            GameManager.Instance.CheckGameOver(activeLetters);
        }

    }

    public void LetterPlaced(GameObject letter)
    {
        if (activeLetters.Contains(letter))
        {
            activeLetters.Remove(letter);
        }


        if (activeLetters.Count == 0)
        {
            SpawnBatch();
        }
        else
        {
            GameManager.Instance.CheckGameOver(activeLetters);
        }
    }

    public List<GameObject> GetActiveLetters() { return activeLetters; }

    public void AssignRandomColor(GameObject letter)
    {
        // random renk seç
        Color chosenColor = letterColors[Random.Range(0, letterColors.Length)];

        // letter prefab’in altındaki cell objelerine uygula
        foreach (Transform child in letter.transform)
        {
            if (child.CompareTag("letterCell"))
            {
                var sr = child.GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.color = chosenColor;
            }
        }

        // bu rengi letter’ın script’inde sakla (ileride grid cell’e aktarmak için)
        letter.GetComponent<DragDrop>().assignedColor = chosenColor;
    }

    public void ClearLetters()
    {
        foreach (var letter in activeLetters)
        {
            if (letter != null)
            {
                Destroy(letter.gameObject);
            }
        }
        activeLetters.Clear();
    }
    
}
