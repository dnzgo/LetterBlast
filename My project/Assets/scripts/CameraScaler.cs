using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    public Camera cam;
    public RectTransform gameHUDHolder;

    void Awake()
    {

        float aspect = (float)Screen.width / Screen.height;

        if (aspect > 0.7f)
        {
            cam.orthographicSize = 12f;
            gameHUDHolder.sizeDelta = new Vector2(gameHUDHolder.sizeDelta.x, 160f); // sets height to 200
        } 
        else
            cam.orthographicSize = 14f;
    }
}