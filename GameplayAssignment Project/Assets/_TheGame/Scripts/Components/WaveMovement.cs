using UnityEngine;

public class WaveMovement : MonoBehaviour
{ // COMPONENT THAT USES COSINE AND SINE TO MOVE OBJECTS IN A WAVEY MANNER

    [SerializeField] float amplitude = 0.5f;
    [SerializeField] float frequency = 1f;
    [SerializeField] float noiseStrength = 0.25f;
    [SerializeField] Vector3 axis = Vector3.up;

    Vector3 startPosition;
    float timeOffset;
    float noiseSeed;

    void Start()
    {
        startPosition = transform.position;
        timeOffset = Random.Range(0f, 100f);
        noiseSeed = Random.Range(0f, 100f);
    }

    void Update()
    {
        float time = Time.time + timeOffset;

        float baseWave = Mathf.Sin(time * frequency);
        float secondaryWave = Mathf.Cos(time * frequency * 0.63f + 1.3f);

        float noise = Mathf.PerlinNoise(noiseSeed, time * 0.5f) - 0.5f;

        float combined = baseWave * 0.6f + secondaryWave * 0.4f + noise * noiseStrength;

        transform.position = startPosition + axis.normalized * combined * amplitude;
    }
}