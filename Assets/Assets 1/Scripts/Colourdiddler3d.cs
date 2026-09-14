using UnityEngine;

public class RainbowColorCycle : MonoBehaviour
{
    [Tooltip("Speed at which the hue cycles through the rainbow.")]
    public float colorChangeSpeed = 1f;

    private Renderer rend;
    private float hue = 0f;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        // Increment hue over time
        hue += colorChangeSpeed * Time.deltaTime;
        if (hue > 1f)
            hue -= 1f;

        // Convert HSV → RGB
        Color rainbowColor = Color.HSVToRGB(hue, 1f, 1f);

        // Apply to material
        rend.material.color = rainbowColor;
    }
}