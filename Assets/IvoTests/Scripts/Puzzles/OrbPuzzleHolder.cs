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
        if (other.GetComponent<OrbPuzzleOrb>() != null)
        {
            if(other.GetComponent<OrbPuzzleOrb>().color == color)
            {
                AttachOrb(other.transform);
            }
        }
    }

    void AttachOrb(Transform orb)
    {
        orb.parent = gameObject.transform;
        orb.GetComponent<XRGrabInteractable>().enabled = false;
        orb.GetComponent<SphereCollider>().enabled = false;
        orb.GetComponent<OrbPuzzleOrb>().Placed(gameObject);
        manager.steps++;
        ac.Play();
        foreach (GameObject particle in particles) 
        {
            particle.SetActive(true);
        }
        manager.CheckCompletion();
    }
}
