using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] letterPrefabs;
    public float yPos = -4f;
    public float xSpacing = 2f;

    private List<GameObject> activeLetters = new List<GameObject>();

    void Start()
    {
        SpawnBatch();
    }

    void SpawnBatch()
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
}
