using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAvatarController : MonoBehaviour
{
    [Header("Head Variants")]
    [Tooltip("Head mesh GameObjects, index-matched.")]

    [SerializeField] private List<GameObject> headVariants;

    private int _headIndex;

    private void Awake()
    {
        
    }

    /// <summary>
    /// Call this locally (e.g. from your customization UI) as the owner.
    /// </summary>
    public void RequestHeadChange(int newIndex)
    {               
        headVariants = GetComponentInChildren<HeadMeshList>(true).headMeshes;

        if (headVariants == null || newIndex < 0 || newIndex >= headVariants.Count)
        {
            Debug.LogWarning($"Invalid head index requested: {newIndex}");
            return;
        }
        RequestHeadChangeServerRpc(newIndex);
    }
    private void RequestHeadChangeServerRpc(int newIndex)
    {
        // Optional: server-side validation here
        _headIndex = newIndex;

        ApplyHeadIndex(newIndex);
    }

    private void ApplyHeadIndex(int index)
    {
        if (headVariants == null)
            return;

        for (int i = 0; i < headVariants.Count; i++)
        {
            if (headVariants[i] != null)
                headVariants[i].SetActive(i == index);
        }
    }
}
