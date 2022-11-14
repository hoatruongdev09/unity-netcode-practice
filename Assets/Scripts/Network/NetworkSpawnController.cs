using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;
using System;

public class NetworkSpawnController : NetworkBehaviour
{
    public static NetworkSpawnController Instance { get; private set; }
    public static OnNetworkObjectClientSpawn onNetworkObjectClientSpawn;
    public static OnNetworkObjectClientDespawn onNetworkObjectClientDespawn;
    [SerializeField] private GameObject character;
    [SerializeField] private GameObject figureCharacter;
    [SerializeField] private NetworkObject propHolderPrefab;

    private NetworkObject characterHolder;

    private Dictionary<ulong, NetworkCharacterController> serverCharacters = new Dictionary<ulong, NetworkCharacterController>();
    private Dictionary<ulong, AttackableUnit> serverUnits = new Dictionary<ulong, AttackableUnit>();
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
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            if (!characterHolder)
            {
                characterHolder = Instantiate(propHolderPrefab);
                characterHolder.Spawn(true);
                characterHolder.name = "Character Holder";
            }
            // var characterData = NetworkContentManager.Instance.GetCharacterData(1);
            // var attackableUnit = CreateFigureUnit(characterData, new Vector3(25, 0, 25));
        }
    }

    public void RemoveObjectsOwnedBy(ulong ownerClientId)
    {
        if (IsServer)
        {
            serverCharacters.Remove(ownerClientId);
            var allServerUnitsKey = serverUnits.Keys.ToArray();
            if (allServerUnitsKey.Length != 0)
            {
                foreach (var key in allServerUnitsKey)
                {
                    if (serverUnits[key].NetworkOwnerID == ownerClientId)
                    {
                        if (serverUnits[key].NetworkObject.IsSpawned)
                        {
                            serverUnits[key].NetworkObject.Despawn();
                        }
                        serverUnits.Remove(key);
                    }
                }
            }
        }
        if (IsClient)
        {

        }
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
    public void CreateCharacterServerRpc(int characterDataId, ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        if (serverCharacters.ContainsKey(clientId)) { return; }
        Vector3 position = NetworkMapController.Instance.GetRandomPositionOnMap();
        var characterData = NetworkContentManager.Instance.GetCharacterData(characterDataId);
        var attackableUnit = CreateUnit(characterData, position);
        attackableUnit.NetworkObject.ChangeOwnership(clientId);
        attackableUnit.NetworkOwnerID = clientId;

        var networkCharacter = attackableUnit.GetComponent<NetworkCharacterController>();
        serverCharacters.Add(clientId, networkCharacter);
        OnMainCharacterSpawnClientRpc(attackableUnit.NetworkObject.NetworkObjectId, attackableUnit.NetworkObject.OwnerClientId);
    }
    private AttackableUnit CreateUnit(ICharacterData characterData, Vector3 position)
    {
        var newCharacter = Instantiate(character, position, Quaternion.Euler(0, Mathf.Atan2(position.x, position.z), 0), characterHolder.transform);
        newCharacter.GetComponent<NetworkObject>().Spawn();
        var attackableUnit = newCharacter.GetComponent<AttackableUnit>();
        attackableUnit.LoadCharacterData(characterData);
        attackableUnit.InitializeStats(characterData.StatsData);
        attackableUnit.InitializeSpells(characterData.GetAllSpells().Select(spellData => GameHelper.CreateSpell(spellData)).ToArray());
        attackableUnit.SetClanId(GameHelper.FREE_CLAN_ID);
        serverUnits.Add(attackableUnit.NetworkObjectId, attackableUnit);
        attackableUnit.NetworkObject.TrySetParent(characterHolder);
        return attackableUnit;
    }

    private AttackableUnit CreateFigureUnit(ICharacterData characterData, Vector3 position)
    {
        var newCharacter = Instantiate(figureCharacter, position, Quaternion.Euler(0, Mathf.Atan2(position.x, position.z), 0), characterHolder.transform);
        newCharacter.GetComponent<NetworkObject>().Spawn();
        var attackableUnit = newCharacter.GetComponent<AttackableUnit>();
        attackableUnit.LoadCharacterData(characterData);
        attackableUnit.InitializeStats(characterData.StatsData);
        attackableUnit.InitializeSpells(characterData.GetAllSpells().Select(spellData => GameHelper.CreateSpell(spellData)).ToArray());
        attackableUnit.SetClanId(GameHelper.FREE_CLAN_ID);
        serverUnits.Add(attackableUnit.NetworkObjectId, attackableUnit);
        attackableUnit.NetworkObject.TrySetParent(characterHolder);
        return attackableUnit;
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

    public AttackableUnit[] FindNearByUnit(TargetFilterParameters parameters)
    {
        Vector3 testDirect;
        float testAngle;
        List<AttackableUnit> units = new List<AttackableUnit>();
        foreach (var unit in serverUnits.Values)
        {
            testDirect = unit.Position - parameters.Position;
            if (testDirect.sqrMagnitude > Mathf.Pow(parameters.Range, 2)) { continue; }
            if (!SpellHelper.IsValidTarget(parameters.SourceUnit, unit, parameters.Filter)) { continue; }
            testAngle = Vector3.Angle(testDirect, parameters.Direct);
            if (testAngle > parameters.Angle) { continue; }
            units.Add(unit);
        }
        return units.ToArray();
    }
    public AttackableUnit FindNearestUnit(TargetFilterParameters parameters)
    {
        AttackableUnit nearestUnit = null;
        Vector3 testDirect;
        Vector3 directFromNearest;
        float testAngle;
        foreach (var unit in serverUnits.Values)
        {
            testDirect = unit.Position - parameters.Position;
            if (testDirect.sqrMagnitude > Mathf.Pow(parameters.Range, 2)) { continue; }
            testAngle = Vector3.Angle(testDirect, parameters.Direct);
            if (testAngle > parameters.Angle) { continue; }
            if (!SpellHelper.IsValidTarget(parameters.SourceUnit, unit, parameters.Filter)) { continue; }
            if (nearestUnit == null)
            {
                nearestUnit = unit;
                continue;
            }
            directFromNearest = nearestUnit.Position - parameters.Position;
            if (directFromNearest.sqrMagnitude > testDirect.sqrMagnitude)
            {
                nearestUnit = unit;
            }
        }
        return nearestUnit;
    }

}
