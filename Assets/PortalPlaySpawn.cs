using UnityEngine;

public class PortalPlaySpawn : MonoBehaviour
{

    [SerializeField] Animator anim;
    

    void OnEnable()
    {
        anim.SetTrigger("Spawn");
    }
    
}
