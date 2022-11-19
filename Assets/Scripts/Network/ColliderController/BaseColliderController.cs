using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BaseColliderController : NetworkBehaviour
{
    [SerializeField] private Collider[] triggerColliders;
    [SerializeField] private Collider[] collisionColliders;
    public virtual void TurnOffColliders()
    {
        var colliders = new List<Collider>();
        colliders.AddRange(triggerColliders);
        colliders.AddRange(collisionColliders);
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }
    }
    public virtual void TurnOnCollider()
    {
        var colliders = new List<Collider>();
        colliders.AddRange(triggerColliders);
        colliders.AddRange(collisionColliders);
        foreach (var collider in colliders)
        {
            collider.enabled = true;
        }
    }
}