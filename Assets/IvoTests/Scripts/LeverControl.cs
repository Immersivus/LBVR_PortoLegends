using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class LeverControl : MonoBehaviour
{
    public AudioSource ac;
    private HingeJoint lever;
    private XRGrabInteractable interactable;
    public UnityEvent leverEvent;

    private bool safetyBool;

    public bool notDoubleSided;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        interactable = GetComponent<XRGrabInteractable>();
        lever = GetComponent<HingeJoint>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!safetyBool)
        {
            if (notDoubleSided)
            {
                if (lever.angle > lever.limits.max - 10)
                {
                    OnLeverMove();
                    safetyBool = true;
                    ac.Play();
                    interactable.enabled = false;
                }
            }
            else 
            {
                if (lever.angle < lever.limits.min || lever.angle > lever.limits.max)
                {
                    OnLeverMove();
                    safetyBool = true;
                    ac.Play();
                    interactable.enabled = false;
                }
            }
        }


    }

    void OnLeverMove()
    { 
        leverEvent?.Invoke();
    }
}
