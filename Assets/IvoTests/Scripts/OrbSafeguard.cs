using System.Collections.Generic;
using UnityEngine;

public class OrbSafeguard : MonoBehaviour
{

    [SerializeField] private List<GameObject> orbs;

    private List<Vector3> orbsStartingPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetStartingPositions();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GetStartingPositions()
    {
        orbsStartingPosition.Clear();
        foreach (GameObject orb in orbs)
        {
            orbsStartingPosition.Add(orb.transform.localPosition);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<OrbPuzzleOrb>() != null)
        {
            other.transform.localPosition = orbsStartingPosition[other.GetComponent<OrbPuzzleOrb>().index];
        }
    }
}
