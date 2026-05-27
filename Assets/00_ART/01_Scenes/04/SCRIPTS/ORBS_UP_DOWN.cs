using UnityEngine;

public class RandomFloat : MonoBehaviour
{
    public float minHeight = 0.5f;
    public float maxHeight = 2f;

    public float minSpeed = 0.5f;
    public float maxSpeed = 2f;

    private Vector3 startPos;
    private float height;
    private float speed;
    private float offset;

    void Start()
    {
        // IMPORTANT: localPosition
        startPos = transform.localPosition;

        height = Random.Range(minHeight, maxHeight);
        speed = Random.Range(minSpeed, maxSpeed);
        offset = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        float y = Mathf.Sin(Time.time * speed + offset) * height;

        // IMPORTANT: localPosition
        transform.localPosition = new Vector3(
            startPos.x,
            startPos.y + y,
            startPos.z
        );
    }
}