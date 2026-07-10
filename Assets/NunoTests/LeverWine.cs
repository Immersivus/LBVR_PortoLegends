using UnityEngine;
using UnityEngine.Playables;

public class LeverWine : MonoBehaviour
{
    public GameObject timelineObject;
    private float rotationThreshold = 0.05f;

    float lastY;

    bool isRotating;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lastY = transform.localEulerAngles.y;
        timelineObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Rigidbody>().angularVelocity.y != 0f)
        {
            isRotating = true;
        }
        else 
        {
            isRotating = false;
        }

        if (isRotating)
        {
            if (!timelineObject.activeSelf)
                timelineObject.SetActive(true);
        }
        else
        {
            if (timelineObject.activeSelf)
            {
                timelineObject.SetActive(false);
            }
        }
    }
}
