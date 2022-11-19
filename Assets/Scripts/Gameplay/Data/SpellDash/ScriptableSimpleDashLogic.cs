using UnityEngine;
using Cysharp.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using System;

[CreateAssetMenu(fileName = "ScriptableSimpleDashLogic", menuName = "Data/Spell/Dash/ScriptableSimpleDashLogic", order = 0)]
public class ScriptableSimpleDashLogic : ADashLogic
{

    public override async void Move(InGameObject target, IDashWorker worker, Vector3 direct, Action<InGameObject> onDashStart, Action<InGameObject> onDashFinish)
    {
        onDashStart?.Invoke(target);
        target.MovementController.StartDashing();
        var currentTime = worker.DashTime;
        while (currentTime >= 0)
        {
            if (!target.IsAlive())
            {
                onDashFinish?.Invoke(target);
                return;
            }
            if (worker.FaceToDirect)
            {
                target.MovementController.RotateTo(0, GameHelper.Angle(direct), 0);
            }
            target.MovementController.MoveDirect(direct, worker.DashSpeed, true);
            await UniTask.Yield();
            currentTime -= Time.deltaTime;
        }
        Debug.Log("dash done");
        target.MovementController.StopDashing();

        onDashFinish?.Invoke(target);
    }


}