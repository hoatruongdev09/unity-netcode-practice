using System;
using UnityEngine;

public interface IDashLogic
{
    void StartLogic(InGameObject target, IDashWorker worker, Vector3 direct, IDashColliderFeedback dashColliderFeedback = null, Action<InGameObject> onDashStart = null, Action<InGameObject> onDashFinish = null);
    void Stop(InGameObject target, IDashWorker worker, IDashColliderFeedback dashColliderFeedback = null);
}