using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cysharp.Threading.Tasks;
using Unity.Netcode.Components;

public class NetworkMapController : NetworkBehaviour
{
    public static NetworkMapController Instance { get; private set; }
    public bool ServerInitialized => serverInitialized && IsServer;
    public bool ClientInitialized => clientInitialized && IsClient;
    [SerializeField] private Vector2 mapSize;
    [SerializeField] private NetworkMapProp propPrefab;
    [SerializeField] private NetworkObject propHolderPrefab;
    [SerializeField] private GameObject ground;

    private NetworkObject mapPropHolder;
    private bool serverInitialized = false;
    private bool clientInitialized = false;
    private Dictionary<string, GameObject> propDictionary = new Dictionary<string, GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        Instance = null;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }



    public Vector3 GetRandomPositionOnMap()
    {
        return new Vector3(Random.Range(0f, mapSize.x), 0, Random.Range(0f, mapSize.y));
    }

    public async UniTask<GameObject> GetPrefabAtClient(string hash)
    {
        await UniTask.WaitUntil(() => ClientInitialized);
        if (propDictionary.TryGetValue(hash, out var prefab))
        {
            return prefab;
        }
        else
        {
            return null;
        }
    }


}