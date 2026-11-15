using UnityEngine;

[RequireComponent(typeof(Cloth))]
public class ClothWind : MonoBehaviour
{
    [SerializeField] float strength = 2f;
    [SerializeField] float speed = 0.2f; 
    [SerializeField] float verticalAmount = 0.2f;

    Cloth cloth;
    float seed;

    void Awake()
    {
        cloth = GetComponent<Cloth>();
        seed = Random.Range(0f, 1000f);
    }

    void Update()
    {
        float t = Time.time * speed;

        // PerlinNoise gives 0..1 that drifts smoothly; remap to -1..1
        float x = (Mathf.PerlinNoise(t, seed) - 0.5f) * 2f;
        float z = (Mathf.PerlinNoise(t, seed + 100f) - 0.5f) * 2f;
        float y = (Mathf.PerlinNoise(t, seed + 200f) - 0.5f) * 2f * verticalAmount;

        cloth.externalAcceleration = new Vector3(x, y, z) * strength;
    }
}