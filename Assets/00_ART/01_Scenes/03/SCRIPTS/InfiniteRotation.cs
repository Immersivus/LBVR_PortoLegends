using UnityEngine;

public class RotateObject : MonoBehaviour
{
    // Rotation speed in degrees per second
    public Vector3 rotationSpeed = new Vector3(0f, 50f, 0f);

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}