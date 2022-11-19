using UnityEngine;

public interface IRocketWorker
{
    int ID { get; }
    GameObject Model { get; }
    Vector3 ModelOffset { get; }
    float Radius { get; }
    Vector3 BoxSize { get; }
    float LifeTime { get; }
    float Speed { get; }
    int LimitCharacterTime { get; }

    IRocketColliderCreator ColliderCreator { get; }
    IRocketMoveLogic GetMoveLogic();
}