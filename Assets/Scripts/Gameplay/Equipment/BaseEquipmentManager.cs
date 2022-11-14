using UnityEngine;
using Unity.Netcode;
using System;

public class BaseEquipmentManager : NetworkBehaviour, IEquipmentManager
{
    public int LeftHandEquipment { get => leftHandEquipment?.Value ?? 0; set => SetLeftHandEquipment(value); }
    public int RightHandEquipment { get => rightHandEquipment?.Value ?? 0; set => SetRightHandEquipment(value); }
    [SerializeField] private ABoneManager boneManager;
    [SerializeField] protected NetworkVariable<int> leftHandEquipment = new NetworkVariable<int>(GameHelper.UNSET_ITEM_ID, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] protected NetworkVariable<int> rightHandEquipment = new NetworkVariable<int>(GameHelper.UNSET_ITEM_ID, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


    private GameObject leftHandEquipmentObject;
    private GameObject rightHandEquipmentObject;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsClient)
        {
            OnClientNetworkSpawn();
            ShowEquipments();
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (IsClient)
        {
            OnClientNetworkDespawn();
        }
    }

    private void OnClientNetworkSpawn()
    {
        leftHandEquipment.OnValueChanged += OnLeftHandEquipmentChanged;
        rightHandEquipment.OnValueChanged += OnRightHandEquipmentChanged;
    }
    private void OnClientNetworkDespawn()
    {
        leftHandEquipment.OnValueChanged -= OnLeftHandEquipmentChanged;
        rightHandEquipment.OnValueChanged -= OnRightHandEquipmentChanged;
    }


    private void ShowEquipments()
    {
        if (LeftHandEquipment != GameHelper.UNSET_ITEM_ID)
        {
            EquipEquipment(LeftHandEquipment, ref leftHandEquipmentObject);
        }
        if (RightHandEquipment != GameHelper.UNSET_ITEM_ID)
        {
            EquipEquipment(RightHandEquipment, ref rightHandEquipmentObject);
        }
    }

    private void SetLeftHandEquipment(int value)
    {
        if (IsServer)
        {
            leftHandEquipment.Value = value;
        }
        if (IsClient)
        {
            SetLeftHandEquipmentServerRpc(value);
        }
    }
    private void SetRightHandEquipment(int value)
    {
        if (IsServer)
        {
            rightHandEquipment.Value = value;
        }
        if (IsClient)
        {
            SetRightHandEquipmentServerRpc(value);
        }
    }
    [ServerRpc(Delivery = RpcDelivery.Reliable)]
    private void SetLeftHandEquipmentServerRpc(int value)
    {
        leftHandEquipment.Value = value;
    }
    [ServerRpc(Delivery = RpcDelivery.Reliable)]
    private void SetRightHandEquipmentServerRpc(int value)
    {
        rightHandEquipment.Value = value;
    }


    private void OnLeftHandEquipmentChanged(int previousValue, int newValue)
    {
        if (newValue != previousValue)
        {
            Debug.Log($"remove left item graphic");
        }
        if (newValue == GameHelper.UNSET_ITEM_ID)
        {
            return;
        }
        EquipEquipment(newValue, ref leftHandEquipmentObject);
    }

    private void OnRightHandEquipmentChanged(int previousValue, int newValue)
    {
        if (newValue != previousValue)
        {
            Debug.Log($"remove left item graphic");
        }
        if (newValue == GameHelper.UNSET_ITEM_ID)
        {
            return;
        }
        EquipEquipment(newValue, ref rightHandEquipmentObject);
    }

    private void EquipEquipment(int id, ref GameObject equipmentObj)
    {
        var equipData = NetworkContentManager.Instance.GetEquipmentData(id);
        if (equipData == null) { return; }
        var bone = boneManager.GetBoneByName(equipData.AttachBone);
        equipmentObj = Instantiate(equipData.Model, bone);
        equipmentObj.transform.localPosition = equipData.OffsetPosition;
        equipmentObj.transform.localRotation = equipData.OffsetRotation;
    }

}