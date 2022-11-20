using UnityEngine;
using Cysharp.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using System;

[CreateAssetMenu(fileName = "ScriptableSimpleDashLogic", menuName = "Data/Spell/Dash/ScriptableSimpleDashLogic", order = 0)]
public class ScriptableSimpleDashLogic : ADashLogic
{

    public override async void StartLogic(InGameObject target, IDashWorker worker, Vector3 direct, IDashColliderFeedback feedback = null, Action<InGameObject> onDashStart = null, Action<InGameObject> onDashFinish = null)
    {
        onDashStart?.Invoke(target);
        target.MovementController.StartDashing();
        var currentTime = worker.DashTime - 0.01f;
        while (currentTime >= 0)
        {
            if (!target.IsAlive())
            {
                feedback?.Remove();
                onDashFinish?.Invoke(target);
                target.MovementController.StopDashing();
                return;
            }
            feedback?.OnUpdate(Time.deltaTime);
            if (worker.FaceToDirect)
            {
                target.MovementController.RotateTo(0, GameHelper.Angle(direct), 0);
            }
            target.MovementController.MoveDirect(direct, worker.DashSpeed, true);
            await UniTask.Yield();
            currentTime -= Time.deltaTime;
        }
        await UniTask.Delay(TimeSpan.FromMilliseconds(100));
        Debug.Log("dash done");
        feedback?.Remove();
        target.MovementController.StopDashing();
        if (target.MovementController.IsStopMovement)
        {
            target.MovementController.StopMovement();
        }
        else
        {
            target.MovementController.StartMovement();
        }
        onDashFinish?.Invoke(target);
    }

    public override void Stop(InGameObject target, IDashWorker worker, IDashColliderFeedback feedback = null)
    {
        feedback?.Remove();
        target.MovementController.StopDashing();
    }
}