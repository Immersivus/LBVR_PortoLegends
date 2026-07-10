using UnityEngine;
using UnityEngine.Playables;

public class LeverWine : MonoBehaviour
{
    public GameObject timelineObject;
    public PlayableDirector timeline;
    private float rotationThreshold = 0.5f;

    float lastY;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lastY = transform.localEulerAngles.y;
        timelineObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        float currentY = transform.localEulerAngles.y;
        bool isRotating = Mathf.Abs(Mathf.DeltaAngle(lastY, currentY)) > rotationThreshold;

        if (isRotating)
        {
            if (!timelineObject.activeSelf)
                timelineObject.SetActive(true);

            if (timeline.state != PlayState.Playing)
                timeline.Play();
        }
        else
        {
            if (timelineObject.activeSelf)
            {
                timeline.Stop();
                timelineObject.SetActive(false);
            }
        }

        lastY = currentY;
    }
}
