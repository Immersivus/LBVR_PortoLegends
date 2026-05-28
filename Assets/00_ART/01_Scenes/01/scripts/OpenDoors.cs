using UnityEngine;

public class OpenDoors : MonoBehaviour
{
    public Animator doorL;
    public Animator doorR;
    public Animator doorGlassL;
    public Animator doorGlassR;

    private bool isOpen = false;

    void OnTriggerEnter (Collider other)
    {
        if (other.CompareTag("Player") && !isOpen)
        {
            doorL.SetTrigger("Open");
            doorR.SetTrigger("Open");
            doorGlassL.SetTrigger("Open");
            doorGlassR.SetTrigger("Open");
            isOpen = true;
        } 
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player") && isOpen)
        {
            doorL.SetTrigger("Close");
            doorR.SetTrigger("Close");
            doorGlassL.SetTrigger("Close");
            doorGlassR.SetTrigger("Close");
        }
    }
}
