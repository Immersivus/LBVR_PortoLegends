using System.Collections.Generic;
using UnityEngine;

public class OrbPool : MonoBehaviour
{
    public GameObject orbPrefab;
    public Transform originPoint;
    public int poolSize = 1;

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject orb = Instantiate(orbPrefab, originPoint.position, originPoint.rotation);
            orb.SetActive(true);

            OrbReturn returnScript = orb.GetComponent<OrbReturn>();
            returnScript.pool = this;

            pool.Enqueue(orb);
        }
    }

    void Update()
    {

    }

     public void ReturnOrb(GameObject orb)
    {
        Rigidbody rb = orb.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        orb.SetActive(false);
        orb.transform.position = originPoint.position;
        orb.transform.rotation = originPoint.rotation;
        orb.SetActive(true);

        pool.Enqueue(orb);
    }
}