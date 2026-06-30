using System.Collections;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{

    [SerializeField] Transform playerTransform;

    [SerializeField] bool tracking;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        }
        else
        {
            if (!tracking)
            {
                tracking = true;
            }

            Vector3 newPosition = new Vector3(playerTransform.position.x, 0, playerTransform.position.z);
            transform.position = newPosition;
        }
    }
}
