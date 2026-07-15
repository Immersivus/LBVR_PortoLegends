using UnityEngine;

public class FloatAndWiggle : MonoBehaviour
{
    [Header("Movement")]
    public float floatHeight = 0.25f;
    public float floatSpeed = 1f;

    [Header("Rotation")]
    public float rotationAngle = 5f;
    public float rotationSpeed = 0.8f;

    [Header("Seed")]
    [Tooltip("Uses the object's sibling index as the seed.")]
    public bool useSiblingIndex = true;

    [Tooltip("Override the automatic seed.")]
    public bool useCustomSeed = false;

    public string customSeed = "MySeed";

    private Vector3 startPosition;
    private Quaternion startRotation;
    private float phaseOffset;

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;

        // Determine seed
        int seed;

        if (useCustomSeed)
        {
            seed = customSeed.GetHashCode();
        }
        else if (useSiblingIndex)
        {
            seed = transform.GetSiblingIndex();
        }
        else
        {
            seed = gameObject.GetInstanceID();
        }

        // Convert seed into a phase offset (0 - 2π)
        System.Random rng = new System.Random(seed);
        phaseOffset = (float)(rng.NextDouble() * Mathf.PI * 2f);
    }

    void Update()
    {
        float t = Time.time;

        // Float
        float yOffset = Mathf.Sin(t * floatSpeed + phaseOffset) * floatHeight;
        transform.position = startPosition + Vector3.up * yOffset;

        // Rotate (slightly different phase so it feels more natural)
        float zRotation = Mathf.Sin(t * rotationSpeed + phaseOffset + 1.3f) * rotationAngle;
        transform.rotation = startRotation * Quaternion.Euler(0f, 0f, zRotation);
    }
}