using System;
using System.Collections.Generic;
using UnityEngine;

public class DashColliderFeedback : MonoBehaviour, IDashColliderFeedback
{

    public Action<IDashColliderFeedback, Collider> OnTriggerCallback { get; set; }

    public Action<IDashColliderFeedback, Collision> OnCollisionCallback { get; set; }

    private List<GameObject> collidedObjects = new List<GameObject>();

    private InGameObject target;

    public void OnUpdate(float delta)
    {
        if (!target) { return; }
        transform.position = target.Position;
    }

    public void Remove()
    {
        collidedObjects.Clear();
        Destroy(gameObject);
    }


    public void SetTarget(InGameObject target)
    {
        this.target = target;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (collidedObjects.Contains(other.gameObject)) { return; }
        OnCollisionCallback?.Invoke(this, other);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (collidedObjects.Contains(other.gameObject)) { return; }
        OnTriggerCallback?.Invoke(this, other);
    }
}