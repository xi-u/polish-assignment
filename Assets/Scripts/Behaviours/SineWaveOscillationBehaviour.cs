using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineWaveOscillationBehaviour : MonoBehaviour
{
    private SpriteRenderer[] spriteRenderers;
    private Color originalColor;
    private Color colorTo;

    [SerializeField]
    private float minAlpha = 0f, maxAlpha = 0.7f, frequency = 1.0f;

    // Update is called once per frame
    private void Update()
    {
        float sineWave = Mathf.Sin(Time.time * frequency) * 0.5f + 0.5f; // Returns a value between 0 and 1
        float delta = Mathf.Lerp(minAlpha, maxAlpha, sineWave);

        Color32 newColor = Color32.Lerp(originalColor, colorTo, delta);
        
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.color = newColor;
        }
    }

    public void Activate(Color originalColor, Color colorTo, params SpriteRenderer[] spriteRenderers)
    {
        this.spriteRenderers = spriteRenderers;
        this.originalColor = originalColor;
        this.colorTo = colorTo;
    }
}
