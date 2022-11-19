using System;
using UnityEngine;


public abstract class ADashLogic : ScriptableObject, IDashLogic
{
    public abstract void Move(InGameObject target, IDashWorker worker, Vector3 direct, Action<InGameObject> onDashStart, Action<InGameObject> onDashFinish);
}