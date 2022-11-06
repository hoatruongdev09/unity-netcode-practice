using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkObjective : NetworkBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) { return; }
        if (!other.CompareTag("playerCharacter")) { return; }
        var no = other.GetComponent<NetworkCharacterController>();
        ServerGiveDamage(no);
    }


    public void ServerGiveDamage(NetworkCharacterController no)
    {
        if (!IsServer) { return; }
        no.ServerTakeDamage(10);
    }
}
