using UnityEngine;

public interface IDashWorker
{
    float DashTime { get; }
    float DashSpeed { get; }
    bool FaceToDirect { get; }
    bool DashColliderTrigger { get; }

    bool DetectCollisionWhenDash { get; }
    IDashLogic DashLogic { get; }
    IDashColliderWorker ColliderWorker { get; }
}