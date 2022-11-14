using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentData", menuName = "Data/EquipmentData", order = 0)]
public class ScriptableEquipmentData : ScriptableObject, IEquipmentData
{
    [field: SerializeField] public int ID { get; set; }

    [field: SerializeField] public GameObject Model { get; set; }

    [field: SerializeField] public string AttachBone { get; set; }

    [field: SerializeField] public Vector3 OffsetPosition { get; set; }

    [field: SerializeField] public Quaternion OffsetRotation { get; set; }
}