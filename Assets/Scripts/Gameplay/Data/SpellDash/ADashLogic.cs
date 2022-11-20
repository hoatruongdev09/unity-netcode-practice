using System;
using UnityEngine;


public abstract class ADashLogic : ScriptableObject, IDashLogic
{
    public abstract void StartLogic(InGameObject target, IDashWorker worker, Vector3 direct, IDashColliderFeedback feedback = null, Action<InGameObject> onDashStart = null, Action<InGameObject> onDashFinish = null);

    public abstract void Stop(InGameObject target, IDashWorker worker, IDashColliderFeedback dashColliderFeedback = null);
}