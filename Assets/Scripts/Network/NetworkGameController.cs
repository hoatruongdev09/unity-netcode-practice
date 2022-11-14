using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using deVoid.Utils;
using System;

public class NetworkGameController : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            RegisterListeners();
        }
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        if (IsServer)
        {
            RemoveListeners();
        }
    }

    private void RegisterListeners()
    {
        Signals.Get<TakeDamageSignal>().AddListener(OnTakeDamageEvent);
    }

    private void RemoveListeners()
    {
        Signals.Get<TakeDamageSignal>().RemoveListener(OnTakeDamageEvent);
    }

    private void OnTakeDamageEvent(IDamageInfo damageInfo)
    {
        if (!IsServer) { return; }
        damageInfo.Target.TakeDamage(damageInfo);
    }
}
