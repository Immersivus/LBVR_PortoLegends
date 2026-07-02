using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAvatarController : NetworkBehaviour
{
    [Header("Head Variants")]
    [Tooltip("Head mesh GameObjects, index-matched.")]
    [SerializeField] private GameObject[] headVariants;

    private readonly SyncVar<int> _headIndex = new SyncVar<int>(0);

    private void Awake()
    {
        _headIndex.OnChange += OnHeadIndexChanged;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        // Ensures correct state on join/late-join
        ApplyHeadIndex(_headIndex.Value);
    }

    /// <summary>
    /// Call this locally (e.g. from your customization UI) as the owner.
    /// </summary>
    public void RequestHeadChange(int newIndex)
    {
        if (!IsOwner)
            return;

        if (headVariants == null || newIndex < 0 || newIndex >= headVariants.Length)
        {
            Debug.LogWarning($"Invalid head index requested: {newIndex}");
            return;
        }
        RequestHeadChangeServerRpc(newIndex);
    }

    [ServerRpc(RequireOwnership = true)]
    private void RequestHeadChangeServerRpc(int newIndex)
    {
        // Optional: server-side validation here
        _headIndex.Value = newIndex;
    }

    private void OnHeadIndexChanged(int prev, int next, bool asServer)
    {
        ApplyHeadIndex(next);
    }

    private void ApplyHeadIndex(int index)
    {
        if (headVariants == null)
            return;

        for (int i = 0; i < headVariants.Length; i++)
        {
            if (headVariants[i] != null)
                headVariants[i].SetActive(i == index);
        }
    }
}
