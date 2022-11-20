using System;
using UnityEngine;
public interface IDashColliderFeedback : IUpdate
{
    Action<IDashColliderFeedback, Collider> OnTriggerCallback { get; }
    Action<IDashColliderFeedback, Collision> OnCollisionCallback { get; }
    
    void SetTarget(InGameObject target);
    void Remove();
}