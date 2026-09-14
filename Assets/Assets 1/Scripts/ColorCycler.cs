using UnityEngine;
using UnityEngine.UI;

public class ColorCycler : MonoBehaviour
{
    public Image targetImage;
    public float cycleSpeed = 1f;

    private float hue = 0f;

    void Update()
    {
        hue += Time.deltaTime * cycleSpeed;
        if (hue > 1f) hue -= 1f;

        Color newColor = Color.HSVToRGB(hue, 1f, 1f);
        targetImage.color = newColor;
    }
}