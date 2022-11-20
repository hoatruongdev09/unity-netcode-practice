using System;
using UnityEngine;


public abstract class ADashColliderWorker : ScriptableObject, IDashColliderWorker
{

    public abstract IDashColliderFeedback CreateFeedBack(Action<IDashColliderFeedback, Collider> triggerCallback, Action<IDashColliderFeedback, Collision> collisionCallback);
}