using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class LeverControl : MonoBehaviour
{
    
    private HingeJoint lever;
    private XRGrabInteractable interactable;
    public UnityEvent leverEvent;

    private bool safetyBool;
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
            if(lever.angle < lever.limits.min + 10 || lever.angle >  lever.limits.max - 10)
            {
                OnLeverMove();
                safetyBool = true;
            }
        }
    }

    void OnLeverMove()
    { 
        leverEvent?.Invoke();
    }
}
