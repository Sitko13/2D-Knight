using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public Camera cam;
    public float parallaxMultiplier;

    private float startPosX;
    private float length;

    void Start()
    {
        startPosX = transform.position.x;

        // Pre Tiled sprite musÌme pouûiù size.x, nie bounds
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        length = sr.size.x;
    }

    void LateUpdate()
    {
        if (cam == null) return;

        // Parallax posun
        float distance = cam.transform.position.x * parallaxMultiplier;

        // Relativna pozÌcia kamery
        float temp = cam.transform.position.x * (1 - parallaxMultiplier);

        // Posunieme sprite
        transform.position = new Vector3(startPosX + distance, transform.position.y, transform.position.z);

        // INFINITE SCROLLING - spr·vna logika
        if (temp > startPosX + length)
        {
            startPosX -= length;
        }
        else if (temp < startPosX - length)
        {
            startPosX += length;
        }
    }
}
