using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class MyNetworkManager : NetworkManager
{
    private Dictionary<ulong, NetworkPlayerController> players;

    private void Awake()
    {
        players = new Dictionary<ulong, NetworkPlayerController>();
    }


    public void AddPlayer(ulong clientId, NetworkPlayerController playerController)
    {
        players.Add(clientId, playerController);
    }
    public void RemovePlayer(ulong clientId)
    {
        players.Remove(clientId);
    }

    public NetworkPlayerController GetPlayer(ulong clientId)
    {
        if (players.TryGetValue(clientId, out var player))
        {
            return player;
        }
        else
        {
            return null;
        }
    }
}
