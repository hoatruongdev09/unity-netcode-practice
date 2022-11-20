using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableSimpleDashColliderWorker", menuName = "Data/Spell/Dash/ScriptableSimpleDashColliderWorker", order = 0)]
public class ScriptableSimpleDashColliderWorker : ADashColliderWorker
{
    [SerializeField] private DashColliderFeedback prefab;
    public override IDashColliderFeedback CreateFeedBack(Action<IDashColliderFeedback, Collider> triggerCallback, Action<IDashColliderFeedback, Collision> collisionCallback)
    {
        var feedback = Instantiate(prefab);
        feedback.OnTriggerCallback = triggerCallback;
        feedback.OnCollisionCallback = collisionCallback;
        return feedback;
    }
}