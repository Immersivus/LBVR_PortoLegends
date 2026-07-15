using System.Collections;
using UnityEngine;

public class LanguageSystemPlayerFollow : MonoBehaviour
{
    bool tracking;
    Transform playerTransform;

    public static LanguageSystemPlayerFollow Instance { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public PlayerAvatarController _localPlayerAvatar;

    public void SetLocalPlayer(PlayerAvatarController avatar)
    {
        _localPlayerAvatar = avatar;
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

    public void ChangeAvatar(int avatarIndex)
    {
    
        if (_localPlayerAvatar == null)
        {
            Debug.LogWarning("Local player avatar not yet assigned.");
            return;
        }

        _localPlayerAvatar.RequestAvatarChange(avatarIndex);
    }

}
