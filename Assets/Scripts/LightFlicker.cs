using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{
    private Light2D light2D;

    [Header("Flicker Settings")]
    public float baseIntensity = 2f;
    public float flickerMin = 0.95f;
    public float flickerMax = 1.05f;
    public float flickerSpeed = 10f;
    public float randomness = 0.005f; // Extra irregularity

    private float flickerTimer;
    private float noiseSeed;

    private void Awake()
    {
        light2D = GetComponent<Light2D>();
        noiseSeed = Random.Range(0f, 100f); // Unique flicker per light
    }

    private void Update()
    {
        flickerTimer += Time.deltaTime * flickerSpeed;

        // Smooth flicker using Perlin noise
        float noise = Mathf.PerlinNoise(flickerTimer, noiseSeed);
        float intensity = Mathf.Lerp(flickerMin, flickerMax, noise);

        // Add a tiny, randomized offset
        intensity += Random.Range(-randomness, randomness);

        light2D.intensity = baseIntensity * intensity;
    }
}