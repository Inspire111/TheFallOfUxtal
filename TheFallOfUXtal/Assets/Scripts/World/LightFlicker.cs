using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SmoothLightFlicker : MonoBehaviour
{
    public Light2D light2D;
    public float intensityBase = 1.0f;
    public float intensityVariation = 0.2f;
    public float flickerSpeed = 2.0f;

    private float noiseSeed;

    void Start()
    {
        if (light2D == null)
            light2D = GetComponent<Light2D>();

        noiseSeed = Random.Range(0f, 1000f);
    }

    void Update()
    {
        float noise = Mathf.PerlinNoise(noiseSeed, Time.time * flickerSpeed);
        light2D.intensity = intensityBase + (noise - 0.5f) * intensityVariation;
    }
}
