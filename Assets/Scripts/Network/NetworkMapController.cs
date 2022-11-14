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
    [SerializeField] private PrefabSettings[] props;
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
        ServerInit();
        ClientInit();
    }
    private void ServerInit()
    {
        Debug.Log($"Init map");
        if (!IsServer) { return; }
        if (!mapPropHolder)
        {
            mapPropHolder = Instantiate(propHolderPrefab);
            mapPropHolder.Spawn(true);
            mapPropHolder.name = "Prop Holder";
        }
        foreach (var prop in props)
        {
            InitializePrefabs(prop, mapSize);
        }
        serverInitialized = true;
    }
    private void ClientInit()
    {
        if (!IsClient) { return; }
        foreach (var prop in props)
        {
            for (int i = 0; i < prop.prefabs.Length; i++)
            {
                Debug.Log($"init prop: {prop.tag}-{i}");
                propDictionary.TryAdd($"{prop.tag}-{i}", prop.prefabs[i]);
            }
        }
        var groundPosition = ground.transform.position;
        groundPosition.x = mapSize.x / 2;
        groundPosition.z = mapSize.y / 2;
        ground.transform.position = groundPosition;
        var groundScale = ground.transform.localScale;
        groundScale.x = mapSize.x;
        groundScale.y = mapSize.y;
        ground.transform.localScale = groundScale;
        clientInitialized = true;
    }

    private void InitializePrefabs(PrefabSettings prefabSettings, Vector2 mapSize)
    {

        int prefabIndex;
        // NetworkMapProp mapProp;
        Vector3 position;
        for (int i = 0; i < prefabSettings.spawnNumber; i++)
        {
            prefabIndex = Random.Range(0, prefabSettings.prefabs.Length);
            position = GetRandomPositionOnMap();
            var mapProp = Instantiate(propPrefab, position, Quaternion.Euler(0, Mathf.Atan2(position.x, position.z) * Mathf.Rad2Deg, 0), mapPropHolder.transform);
            mapProp.prefabIndexShouldSpawn.Value = prefabIndex;
            mapProp.prefabTag.Value = (int)prefabSettings.tag;
            mapProp.NetworkObject.Spawn(true);
            if (!mapProp.NetworkObject.TrySetParent(mapPropHolder.transform))
            {

            }
        }
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



    [System.Serializable]
    public struct PrefabSettings
    {
        public PrefabTag tag;
        public GameObject[] prefabs;
        public int spawnNumber;

        public PrefabSettings(PrefabTag prefix, GameObject[] prefabs, int spawnNumber)
        {
            this.tag = prefix;
            this.prefabs = prefabs;
            this.spawnNumber = spawnNumber;
        }
    }
}
public enum PrefabTag
{
    Undefined = 0,
    Tree = 1,
    Rock = 2
}
