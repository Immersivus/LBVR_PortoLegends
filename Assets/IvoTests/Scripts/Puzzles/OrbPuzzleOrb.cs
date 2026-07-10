using UnityEngine;

public class OrbPuzzleOrb : MonoBehaviour
{
    public OrbPuzzleHolder.holderColor color;

    private GameObject parent;
    public bool placed;

    public int index;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Rigidbody>().AddForce(Vector3.forward * 10);
    }

    // Update is called once per frame
    void Update()
    {
        if (placed)
        {
            transform.localPosition = Vector3.zero;
            transform.parent = parent.transform;           
        }
    }

    public void Placed(GameObject newParent)
    {
        placed = true;
        parent = newParent;
        GetComponent<Rigidbody>().useGravity = false;
    }
}
