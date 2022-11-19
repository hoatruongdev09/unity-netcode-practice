using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class InGameObject : NetworkBehaviour
{
    [SerializeField] protected NetworkVariable<bool> isDied = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public ulong NetworkOwnerID { get; set; }
    [field: SerializeField] public TeamId Team { get; protected set; }
    [field: SerializeField] public int ClanID { get; protected set; }
    [field: SerializeField] public bool IsRemoved { get; protected set; }
    public BaseAnimationController AnimationController => animationController;
    public BaseMovementController MovementController => movementController;
    public BaseColliderController ColliderController => colliderController;
    public IBoneManager BoneManager => boneManager;
    public Vector3 Forward => transform.forward;
    public Vector3 Position => transform.position;
    public Quaternion Rotation => transform.rotation;
    public bool MovementActive { get; protected set; } = true;
    public bool RotationActive { get; protected set; } = true;
    [SerializeField] private BaseAnimationController animationController;
    [SerializeField] private BaseMovementController movementController;
    [SerializeField] private BaseColliderController colliderController;
    [SerializeField] private ABoneManager boneManager;
    [SerializeField] protected Transform graphicHolder;
    [SerializeField] protected bool isStaticObject;

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


    protected virtual void ServerUpdate()
    {

    }
    protected virtual void ClientUpdate()
    {

    }
    public virtual bool IsAlive()
    {
        return !isDied.Value;
    }
    public virtual void SetDied()
    {
        isDied.Value = true;
    }
    public virtual void SetIsRemoved()
    {
        IsRemoved = true;
    }
    public virtual void SetClanId(int id)
    {
        ClanID = id;
    }
    public virtual void SetTeam(TeamId team)
    {
        Team = team;
    }
    public virtual void Move(Vector3 direct)
    {
        if (!CanMove()) { return; }
        MovementController?.MoveDirect(direct, 1);
    }
    public virtual void Sprint(Vector3 direct)
    {
        if (!CanMove()) { return; }
        Sprint(direct, 1);
    }
    public virtual void Sprint(Vector3 direct, float speed)
    {
        if (!CanMove()) { return; }
        AnimationController?.PlayBoolAnimation("sprint");
        MovementController?.MoveDirect(direct, speed);
    }
    public virtual void Move(Vector3 direct, float speed)
    {
        if (!CanMove()) { return; }
        AnimationController?.PauseBoolAnimation("sprint");
        AnimationController?.PlayBoolAnimation("run");
        MovementController?.MoveDirect(direct, speed);
    }
    public virtual void RotateTo(float x, float y, float z)
    {
        if (!CanRotate()) { return; }
        MovementController?.RotateTo(x, y, z);
    }
    public virtual void RotateTo(float x, float y, float z, float speed)
    {
        if (!CanRotate()) { return; }
        MovementController?.RotateTo(x, y, z, speed);
    }

    public virtual void SetPosition(Vector3 position)
    {
        transform.position = position;
    }
    public virtual void SetRotation(Quaternion rotation)
    {
        transform.rotation = rotation;
    }

    public virtual void StartMovement()
    {
        MovementController?.StartMovement();
    }
    public virtual void StopMovement()
    {
        AnimationController?.PauseBoolAnimation("sprint");
        AnimationController?.PauseBoolAnimation("run");
        MovementController?.StopMovement();
    }

    public virtual bool CanMove()
    {
        return !isStaticObject && MovementActive && MovementController != null;
    }
    public virtual bool CanRotate()
    {
        return RotationActive;
    }
    public virtual void SetActiveMovement(bool movement)
    {
        MovementActive = movement;
        if (!movement)
        {
            AnimationController?.PauseBoolAnimation("sprint");
            AnimationController?.PauseBoolAnimation("run");
        }
    }
    public virtual void SetActiveRotation(bool rotation)
    {
        RotationActive = rotation;
    }

    public virtual void PlayAnimation(string animation, float speed = 1)
    {
        AnimationController.PlayTriggerAnimation(animation, speed);
    }
    public virtual void SetAnimation(string animation, bool value)
    {
        if (value)
        {
            AnimationController?.PlayBoolAnimation(animation);
        }
        else
        {
            AnimationController?.PauseBoolAnimation(animation);
        }
    }
    public virtual void ActiveColliders()
    {
        ColliderController.TurnOnCollider();
    }
    public virtual void DisableColliders()
    {
        ColliderController.TurnOffColliders();
    }
}
