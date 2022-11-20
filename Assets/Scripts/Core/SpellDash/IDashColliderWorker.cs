using System;
using UnityEngine;
public interface IDashColliderWorker
{
    IDashColliderFeedback CreateFeedBack(Action<IDashColliderFeedback, Collider> triggerCallback, Action<IDashColliderFeedback, Collision> collisionCallback);
}