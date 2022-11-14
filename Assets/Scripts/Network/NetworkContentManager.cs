using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class NetworkContentManager : NetworkBehaviour
{
    public static NetworkContentManager Instance { get; private set; }
    public ScriptableCharacterData[] allCharacterData;
    public ScriptableEquipmentData[] allEquipmentData;
    private void Awake()
    {
        if (Instance == null) { Instance = this; }
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        Instance = null;
    }

    public ICharacterData GetCharacterData(int id)
    {
        foreach (var data in allCharacterData)
        {
            if (data.ID == id) { return data; }
        }
        return null;
    }
    public IEquipmentData GetEquipmentData(int id)
    {
        foreach (var data in allEquipmentData)
        {
            if (data.ID == id) { return data; }
        }
        return null;
    }
}
