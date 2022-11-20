using UnityEngine;

[CreateAssetMenu(fileName = "RocketWorker", menuName = "Data/Rocket/RocketWorker", order = 0)]
public class ScriptableRocketWorker : ScriptableObject, IRocketWorker
{
    [field: SerializeField] public int ID { get; set; }

    [field: SerializeField] public GameObject Model { get; set; }

    [field: SerializeField] public Vector3 ModelOffset { get; set; }

    [field: SerializeField] public float Radius { get; set; }

    [field: SerializeField] public Vector3 BoxSize { get; set; }

    [field: SerializeField] public float LifeTime { get; set; }

    [field: SerializeField] public float Speed { get; set; }

    [field: SerializeField] public int LimitCharacterTime { get; set; }

    public IRocketColliderCreator ColliderCreator => colliderCreator;

    [SerializeField] private ARocketColliderCreator colliderCreator;
    [SerializeField] private ARocketMoveLogic rocketMoveLogic;

    public IRocketMoveLogic GetMoveLogic()
    {
        var moveLogic = Object.Instantiate(rocketMoveLogic);
        return moveLogic;
    }
}