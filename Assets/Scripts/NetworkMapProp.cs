using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using System.Threading.Tasks;
using Unity.Collections;

public class NetworkMapProp : NetworkBehaviour
{
    public NetworkVariable<int> prefabIndexShouldSpawn = new NetworkVariable<int>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> prefabTag = new NetworkVariable<int>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsClient)
        {
            SpawnPropAsync((PrefabTag)prefabTag.Value, prefabIndexShouldSpawn.Value);
        }
    }
    private async Task SpawnPropAsync(PrefabTag prefabPrefix, int prefabIndexShouldSpawn)
    {
        var prefab = await NetworkMapController.Instance.GetPrefabAtClient($"{prefabPrefix}-{prefabIndexShouldSpawn}");
        Debug.Log($"Spawn prop: {prefabPrefix} {prefabIndexShouldSpawn} {prefab == null}");
        if (!prefab) { return; }
        Instantiate(prefab, transform.position, transform.rotation, transform);
    }
}
