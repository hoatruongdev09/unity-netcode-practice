using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class NetworkSpawnController : NetworkBehaviour
{
    public static NetworkSpawnController Instance { get; private set; }
    public static OnNetworkObjectClientSpawn onNetworkObjectClientSpawn;
    public static OnNetworkObjectClientDespawn onNetworkObjectClientDespawn;
    [SerializeField] private GameObject character;

    private Dictionary<ulong, NetworkCharacterController> serverCharacters = new Dictionary<ulong, NetworkCharacterController>();
    private Dictionary<ulong, NetworkObject> clientObjects = new Dictionary<ulong, NetworkObject>();

    public delegate void OnNetworkObjectClientSpawn(ulong objectId, NetworkObject networkObject);
    public delegate void OnNetworkObjectClientDespawn(ulong objectId);
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        onNetworkObjectClientSpawn += OnNetworkObjectSpawn;
        onNetworkObjectClientDespawn += OnNetworkObjectDespawn;

    }
    public override void OnDestroy()
    {
        base.OnDestroy();

        onNetworkObjectClientSpawn -= OnNetworkObjectSpawn;
        onNetworkObjectClientDespawn -= OnNetworkObjectDespawn;

    }

    private void OnNetworkObjectDespawn(ulong objectId)
    {
        if (!IsClient) { return; }
        clientObjects.Remove(objectId);
        Debug.Log($"Client remove object: {objectId}");
    }

    private void OnNetworkObjectSpawn(ulong objectId, NetworkObject networkObject)
    {
        if (!IsClient) { return; }
        clientObjects.Add(objectId, networkObject);
        Debug.Log($"Client add object: {objectId}");
    }

    [ServerRpc(RequireOwnership = false)]
    public void CreateCharacterServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        if (serverCharacters.ContainsKey(clientId)) { return; }
        Vector3 position = NetworkMapController.Instance.GetRandomPositionOnMap();
        var newCharacter = Instantiate(character, position, Quaternion.Euler(0, Mathf.Atan2(position.x, position.z), 0));
        var characterNO = newCharacter.GetComponent<NetworkObject>();
        characterNO.SpawnWithOwnership(clientId);
        var networkCharacter = characterNO.GetComponent<NetworkCharacterController>();
        serverCharacters.Add(clientId, networkCharacter);
        OnMainCharacterSpawnClientRpc(characterNO.NetworkObjectId, characterNO.OwnerClientId);
        Debug.Log($"{serverRpcParams.Receive.SenderClientId} {characterNO.OwnerClientId}");
    }
    [ClientRpc]
    private void OnMainCharacterSpawnClientRpc(ulong characterId, ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log($"client receive main character {clientId} {NetworkManager.Singleton.LocalClientId}");
            var character = clientObjects[characterId].GetComponent<NetworkCharacterController>();
            var clientController = (MyNetworkManager.Singleton as MyNetworkManager).GetPlayer(clientId);
            clientController?.SetCharacter(character);
        }
    }
    public void ServerDeleteCharacter(NetworkCharacterController character)
    {
        if (!IsServer) { return; }
        serverCharacters.Remove(character.OwnerClientId);
        character.NetworkObject.Despawn();
    }


}
