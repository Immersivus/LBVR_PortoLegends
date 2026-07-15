using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class PlayerAvatarController : NetworkBehaviour
{
    // Server has authority to write; clients get read-only access + change callback
    private readonly SyncVar<int> _avatarIndex = new SyncVar<int>(
        new SyncTypeSettings(WritePermission.ServerOnly, ReadPermission.Observers));

    [SerializeField] private List<GameObject> avatarPrefabs; // reference set (or you spawn/swap models)

    private void Start()
    {
        
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (IsOwner)
        {
            // Only the local player's own instance registers itself with the UI
            LanguageSystemPlayerFollow.Instance.SetLocalPlayer(this);
        }
    }

    public override void OnStartNetwork()
    {
        _avatarIndex.OnChange += OnAvatarIndexChanged;
    }

    public override void OnStopNetwork()
    {
        _avatarIndex.OnChange -= OnAvatarIndexChanged;
    }

    // Called on every client (and server) whenever the value changes,
    // and also once when a client first receives it (asServer / OnStartNetwork sync).
    private void OnAvatarIndexChanged(int prev, int next, bool asServer)
    {
        Debug.Log(2);
        ApplyAvatarVisual(next);
    }

    private void ApplyAvatarVisual(int index)
    {
        
        if (index >= 0 && index < avatarPrefabs.Count)
        {
            for(int i = 0; i < avatarPrefabs.Count; i++)
            {
                if (i == index)
                {
                    avatarPrefabs[i].SetActive(true);
                }
                else
                {
                    avatarPrefabs[i].SetActive(false);
                }
            }
        }
            
    }

    // --- Client requests a change ---

    // Call this from the owning client's UI / menu logic
    public void RequestAvatarChange(int newIndex)
    {
        Debug.Log(1);
        ChangeAvatarServerRpc(newIndex);
    }

    [ServerRpc(RequireOwnership = true, RunLocally = true)]
    private void ChangeAvatarServerRpc(int newIndex, NetworkConnection sender = null)
    {
        Debug.Log(3);
        avatarPrefabs = GetComponentInChildren<HeadMeshList>(true).headMeshes;
        if (newIndex < 0 || newIndex >= avatarPrefabs.Count)
            return;

        if (IsServerInitialized)
        {
            // Only the server is allowed to write the authoritative SyncVar
            _avatarIndex.Value = newIndex;
        }
        else
        {
            // This is the local/client-side pass — just apply the visual immediately
            ApplyAvatarVisual(newIndex);
        }
    }
}
