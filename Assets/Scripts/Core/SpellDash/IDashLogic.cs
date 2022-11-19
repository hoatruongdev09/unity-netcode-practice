using System;
using UnityEngine;

public interface IDashLogic
{
    void Move(InGameObject target, IDashWorker worker, Vector3 direct, Action<InGameObject> onDashStart, Action<InGameObject> onDashFinish);
}