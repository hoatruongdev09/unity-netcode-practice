using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class NetworkPlayerController : NetworkBehaviour
{
    public NetworkCharacterController CurrentCharacter { get; private set; }
    private void Start()
    {
        NetworkSpawnController.onNetworkObjectClientDespawn += OnObjectDespawn;
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        NetworkSpawnController.onNetworkObjectClientDespawn -= OnObjectDespawn;
    }

    private void OnObjectDespawn(ulong objectId)
    {
        if (IsServer)
        {

        }
        if (IsClient)
        {
            if (CurrentCharacter != null && CurrentCharacter.NetworkObjectId == objectId)
            {
                GameUIManager.Instance.ShowJoinPanel(true);
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log("Network spawning");
        if (IsOwner)
        {
            Debug.Log("Join server !");
            GameUIManager.Instance.ShowJoinPanel(true);
        }
        (MyNetworkManager.Singleton as MyNetworkManager).AddPlayer(this.OwnerClientId, this);
    }
    public void SetCharacter(NetworkCharacterController characterController)
    {
        if (IsClient)
        {
            Debug.Log("Set character at client");
        }
        if (IsServer)
        {
            Debug.Log("Set character at server");
        }
        CurrentCharacter = characterController;
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        (MyNetworkManager.Singleton as MyNetworkManager).RemovePlayer(this.OwnerClientId);
        NetworkSpawnController.Instance.RemoveObjectsOwnedBy(this.OwnerClientId);
    }
}
