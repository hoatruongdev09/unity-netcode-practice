using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using Unity.Netcode.Components;

public class NetworkCharacterController : NetworkBehaviour
{
    [SerializeField] private AttackableUnit controllingUnit;
    private Vector3 moveInput = Vector3.zero;
    private float rotateAngle;
    private bool isMovementPressing;
    private bool isSprintPressing;
    private int[] spellIndexes = new int[] { 1, 2, 3, 4 };
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsClient)
        {
            NetworkSpawnController.onNetworkObjectClientSpawn.Invoke(this.NetworkObjectId, this.NetworkObject);
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (IsOwner)
        {
            CameraController.Instance.SetTarget(null);
        }
        if (IsClient)
        {
            NetworkSpawnController.onNetworkObjectClientDespawn.Invoke(this.NetworkObjectId);
        }
    }

    public override void OnGainedOwnership()
    {
        base.OnGainedOwnership();
        if (IsOwner && IsClient)
        {
            CameraController.Instance.SetTarget(transform);
        }
    }

    private void Start()
    {

    }


    private void Update()
    {

        if (IsServer)
        {
            ServerUpdate();
        }
        if (IsClient)
        {
            ClientUpdate();
        }
    }
    private void FixedUpdate()
    {
        if (IsServer)
        {
            ServerFixedUpdate();
        }
    }

    private void ServerFixedUpdate()
    {
        if (isMovementPressing)
        {
            if (isSprintPressing)
            {
                controllingUnit.Sprint(moveInput);
            }
            else
            {
                controllingUnit.Move(moveInput);
            }
        }
    }

    private void ServerUpdate()
    {
        if (isMovementPressing)
        {
            controllingUnit.RotateTo(0, rotateAngle, 0);
        }
    }

    private void ClientUpdate()
    {
        if (!IsOwner) { return; }
        if (IsMovementInput())
        {
            moveInput.x = Input.GetAxis("Horizontal");
            moveInput.z = Input.GetAxis("Vertical");

            if (!isMovementPressing)
            {
                CommandMoveServerRpc(true);
            }
            RequestMoveServerRpc(moveInput);
            isMovementPressing = true;

        }
        else
        {
            if (isMovementPressing)
            {
                CommandMoveServerRpc(false);
            }
            isMovementPressing = false;
        }

        if (IsAttackInput())
        {
            RequestAttackServerRpc();
        }
        if (IsMainWeaponSelected())
        {
            RequestSelectMainWeaponServerRpc();
        }
        if (IsSprintInput())
        {
            if (!isSprintPressing)
            {
                RequestSprintServerRpc(true);
            }
            isSprintPressing = true;
        }
        else
        {
            if (isSprintPressing)
            {
                RequestSprintServerRpc(false);
            }
            isSprintPressing = false;
        }

        if (IsSpell1CastInput())
        {
            RequestCastSpellServerRpc(spellIndexes[0], Vector3.zero, GameHelper.UNSET_VECTOR_3);
        }
        if (IsSpell2CastInput())
        {
            RequestCastSpellServerRpc(spellIndexes[1], Vector3.zero, GameHelper.UNSET_VECTOR_3);
        }
        if (IsSpell3CastInput())
        {
            RequestCastSpellServerRpc(spellIndexes[2], Vector3.right, GameHelper.UNSET_VECTOR_3);
        }
        if (IsSpell4CastInput())
        {
            RequestCastSpellServerRpc(spellIndexes[3], Vector3.zero, GameHelper.UNSET_VECTOR_3);
        }

    }

    private bool IsMainWeaponSelected()
    {
        return Input.GetKeyDown(KeyCode.Alpha1);
    }
    private bool IsMovementInput()
    {
        return Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow);
    }
    private bool IsAttackInput()
    {
        return Input.GetKey(KeyCode.A);
    }
    private bool IsSpell1CastInput()
    {
        return Input.GetKey(KeyCode.Q);
    }
    private bool IsSpell2CastInput()
    {
        return Input.GetKey(KeyCode.W);
    }
    private bool IsSpell3CastInput()
    {
        return Input.GetKey(KeyCode.E);
    }
    private bool IsSpell4CastInput()
    {
        return Input.GetKey(KeyCode.R);
    }
    private bool IsSprintInput()
    {
        return Input.GetKey(KeyCode.Space);
    }

    [ServerRpc(Delivery = RpcDelivery.Reliable)]
    private void RequestSelectMainWeaponServerRpc(ServerRpcParams serverRpcParams = default)
    {
        controllingUnit?.EquipItem(1);
    }

    [ServerRpc(Delivery = RpcDelivery.Reliable)]
    private void RequestAttackServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var spell = controllingUnit.GetSpell(0);
        if (!controllingUnit.CanCast(spell)) { return; }
        controllingUnit.CastSpell(spell, null, controllingUnit.Forward, GameHelper.UNSET_VECTOR_3);
    }
    [ServerRpc(Delivery = RpcDelivery.Reliable)]
    private void RequestSprintServerRpc(bool value)
    {
        this.isSprintPressing = value;
    }
    [ServerRpc(Delivery = RpcDelivery.Reliable)]
    private void CommandMoveServerRpc(bool value)
    {
        if (value && !this.isMovementPressing)
        {
            controllingUnit.StartMovement();
        }
        else if (this.isMovementPressing && !value)
        {
            controllingUnit.StopMovement();
        }
        this.isMovementPressing = value;
    }
    [ServerRpc(Delivery = RpcDelivery.Unreliable)]
    private void RequestMoveServerRpc(Vector3 moveInput)
    {
        this.moveInput = GameHelper.ModifyDirectByCurrentView(moveInput);
        this.rotateAngle = GameHelper.Angle(this.moveInput);
    }

    [ServerRpc(Delivery = RpcDelivery.Reliable)]
    private void RequestCastSpellServerRpc(int spellIndex, Vector3 direct, Vector3 location)
    {
        if (!controllingUnit) { return; }
        var spell = controllingUnit.GetSpell(spellIndex);
        if (spell == null || !controllingUnit.CanCast(spell)) { return; }
        controllingUnit.CastSpell(spell, null, direct, location);
    }

    public void ServerTakeDamage(float amount)
    {

    }
}
