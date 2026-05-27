using UnityEngine;

public class RandomFloatAndRotate : MonoBehaviour
{
    [Header("Floating Object")]
    public Transform floatingObject;

    [Header("Float Settings")]
    public float minHeight = 0.5f;
    public float maxHeight = 2f;

    public float minSpeed = 0.5f;
    public float maxSpeed = 2f;

    [Header("Rotation Settings")]
    public Vector3 rotationAxis = Vector3.up;
    public float rotationSpeed = 20f;

    private Vector3 startLocalPos;
    private float height;
    private float speed;
    private float offset;

    void Start()
    {
        if (floatingObject == null)
            floatingObject = transform.GetChild(0);

        startLocalPos = floatingObject.localPosition;

        height = Random.Range(minHeight, maxHeight);
        speed = Random.Range(minSpeed, maxSpeed);
        offset = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        // Rotate parent
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);

        // Float child smoothly
        float y = Mathf.Sin(Time.time * speed + offset) * height;

        floatingObject.localPosition = new Vector3(
            startLocalPos.x,
            startLocalPos.y + y,
            startLocalPos.z
        );
    }
}