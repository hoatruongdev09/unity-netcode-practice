using UnityEngine;

public interface IDashWorker
{
    float DashTime { get; }
    float DashSpeed { get; }
    bool FaceToDirect { get; }

    IDashLogic DashLogic { get; }
}