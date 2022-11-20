using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableDashWorker", menuName = "Data/Spell/Dash/ScriptableDashWorker", order = 0)]
public class ScriptableDashWorker : ScriptableObject, IDashWorker
{
    [field: SerializeField] public float DashTime { get; set; }

    [field: SerializeField] public float DashSpeed { get; set; }

    [field: SerializeField] public bool FaceToDirect { get; set; }

    [field: SerializeField] public bool DashColliderTrigger { get; set; }

    [field: SerializeField] public bool DetectCollisionWhenDash { get; set; }

    public IDashLogic DashLogic => dashLogic;

    public IDashColliderWorker ColliderWorker => dashColliderWorker;

    [SerializeField] private ADashLogic dashLogic;
    [SerializeField] private ADashColliderWorker dashColliderWorker;
}