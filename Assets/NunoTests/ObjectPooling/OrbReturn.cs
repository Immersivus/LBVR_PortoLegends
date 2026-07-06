using UnityEngine;

public class OrbReturn : MonoBehaviour
{
    public OrbPool pool;
    public float fallHeight = -2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < fallHeight)
        {
            pool.ReturnOrb(gameObject);
        }
    }
}