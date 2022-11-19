using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using deVoid.Utils;

public class Rocket : InGameObject
{
    [SerializeField] private NetworkVariable<int> workerData = new NetworkVariable<int>(GameHelper.UNSET_ID, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public ICastInfo CastInfo { get; protected set; }
    public IRocketWorker RocketWorker { get; protected set; }
    public Vector3 MoveDirect { get; protected set; }
    public InGameObject Target { get; protected set; }
    public int CurrentCharacterHitTime { get; protected set; }
    private IRocketMoveLogic rocketMoveLogic;
    private GameObject clientCurrentModel;
    private float currentLifeTime;
    private RocketTriggerObject triggerObjectSignal;
    private RocketCollisionEvent collisionObjectSignal;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            triggerObjectSignal = Signals.Get<RocketTriggerObject>();
            collisionObjectSignal = Signals.Get<RocketCollisionEvent>();
            currentLifeTime = 0;
        }
        if (IsClient)
        {
            workerData.OnValueChanged += OnWorkerDataChanged;
            if (workerData.Value != GameHelper.UNSET_ID)
            {
                ShowModels(workerData.Value);
            }
        }
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (IsClient)
        {
            workerData.OnValueChanged -= OnWorkerDataChanged;
        }
    }


    private void OnWorkerDataChanged(int previousValue, int newValue)
    {
        Debug.Log($"on worker changed: {previousValue} {newValue}");
        if (clientCurrentModel) { Destroy(clientCurrentModel); }
        if (newValue == GameHelper.UNSET_ID) { return; }
        ShowModels(newValue);
    }

    private void ShowModels(int value)
    {
        var worker = NetworkContentManager.Instance.GetRocketWorker(workerData.Value);
        if (worker == null) { return; }
        clientCurrentModel = Instantiate(worker.Model, graphicHolder);
        clientCurrentModel.transform.localPosition = worker.ModelOffset;
        clientCurrentModel.transform.localRotation = Quaternion.identity;
    }

    public void SetCurrentHitTime(int value)
    {
        CurrentCharacterHitTime = value;
    }
    public void SetCastInfo(ICastInfo castInfo)
    {
        CastInfo = castInfo;
    }
    public void SetWorker(IRocketWorker worker)
    {
        RocketWorker = worker;
        workerData.Value = worker.ID;
        rocketMoveLogic = worker.GetMoveLogic();
        RocketWorker.ColliderCreator.CreateCollider(worker, this);
    }

    public void SetMoveDirect(Vector3 moveDirect)
    {
        MoveDirect = moveDirect;
    }
    public void SetFollowTarget(InGameObject target)
    {
        Target = target;
    }

    protected override void ServerUpdate()
    {
        if (IsRemoved) { return; }
        if (RocketWorker.LifeTime != -1)
        {
            currentLifeTime = Mathf.Min(currentLifeTime + Time.deltaTime, RocketWorker.LifeTime);
            if (!IsAlive())
            {
                NetworkSpawnController.Instance.RemoveGameObject(NetworkObject.NetworkObjectId);
                return;
            }
        }
        rocketMoveLogic?.Move(this, Time.deltaTime);
    }
    public override bool IsAlive()
    {
        return !isDied.Value && currentLifeTime < RocketWorker.LifeTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {
            if (other.TryGetComponent<InGameObject>(out var obj))
            {
                triggerObjectSignal.Dispatch(this, obj);
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (IsServer)
        {
            collisionObjectSignal.Dispatch(this, other);
        }
    }
}