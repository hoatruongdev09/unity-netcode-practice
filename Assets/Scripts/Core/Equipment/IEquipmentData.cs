using UnityEngine;

public interface IEquipmentData
{
    int ID { get; }
    GameObject Model { get; }
    string AttachBone { get; }
    Vector3 OffsetPosition { get; }
    Quaternion OffsetRotation { get; }
}