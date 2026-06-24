using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using XRoam.Experience.Colliders;

public class OrbPuzzleHolder : MonoBehaviour
{
    public enum holderColor { Yellow, Green, Blue, Red }

    public holderColor color;

    private AudioSource ac;

    [SerializeField] private List<GameObject> particles;

    [SerializeField] private PuzzleManager manager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ac = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<OrbPuzzleHolder>() != null)
        {
            if(other.GetComponent<OrbPuzzleHolder>().color == color)
            {
                AttachOrb(other.transform);
            }
        }
    }

    void AttachOrb(Transform orb)
    {
        orb.parent = this.transform;
        orb.localPosition = Vector3.zero;
        orb.GetComponent<XRGrabInteractable>().enabled = false;
        manager.steps++;
        ac.Play();
        foreach (GameObject particle in particles) 
        {
            particle.SetActive(true);
        }
        manager.CheckCompletion();
    }
}
