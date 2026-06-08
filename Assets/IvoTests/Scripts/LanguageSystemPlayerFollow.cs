using System.Collections;
using UnityEngine;

public class LanguageSystemPlayerFollow : MonoBehaviour
{
    bool tracking;
    Transform playerTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
                StartCoroutine(MoveToPlayer());
                tracking = true;
            }

            Vector3 newPosition = new Vector3(playerTransform.position.x, playerTransform.position.y, playerTransform.position.z);
            transform.position = newPosition;
        }
    }

    IEnumerator MoveToPlayer()
    {
        while (true)
        {
            Quaternion stabilizedRotation = new Quaternion(transform.rotation.x, playerTransform.rotation.y, transform.rotation.z, playerTransform.rotation.w);
            transform.rotation = Quaternion.Lerp(transform.rotation, stabilizedRotation, Time.deltaTime);
            yield return null;
        }
    }
}
