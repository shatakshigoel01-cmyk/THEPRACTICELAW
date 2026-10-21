
using UnityEngine;
using System.Collections;

public class KnifeGlow : MonoBehaviour
{
    public Renderer knifeRenderer;
    public Color glowColor = Color.cyan;
    public float glowDuration = 2f;

    private Material knifeMaterial;
    private Color originalColor;

    void Start()
    {
        if (knifeRenderer == null)
            knifeRenderer = GetComponentInChildren<Renderer>();

        if (knifeRenderer != null)
        {
            knifeMaterial = knifeRenderer.material;
            originalColor = knifeMaterial.color;
        }
    }

    public void GlowOnce()
    {
        if (knifeMaterial != null)
            StartCoroutine(GlowRoutine());
    }

    IEnumerator GlowRoutine()
    {
        knifeMaterial.color = glowColor;
        yield return new WaitForSeconds(glowDuration);
        knifeMaterial.color = originalColor;
    }
}
