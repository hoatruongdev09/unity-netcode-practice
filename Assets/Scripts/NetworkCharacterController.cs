using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using Unity.Netcode.Components;

public class NetworkCharacterController : NetworkBehaviour
{
    [SerializeField] private NetworkVariable<float> currentHp = new NetworkVariable<float>(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] private NetworkVariable<float> moveSpeed = new NetworkVariable<float>(5);
    [SerializeField] private NetworkVariable<float> rotateSpeed = new NetworkVariable<float>(10);
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private NetworkAnimator networkAnimator;
    [SerializeField] private CharacterController characterController;

    private Vector3 moveInput = Vector3.zero;
    private float horizontal = 0;
    private float vertical = 0;
    private bool isSendZeroInput = false;

    private Camera mainCamera;
    private Vector3 position;
    private void Awake()
    {
        mainCamera = Camera.main;
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            CameraController.Instance.SetTarget(transform);
        }
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

    private void Start()
    {
        currentHp.OnValueChanged += OnHpChanged;
    }


    private void OnHpChanged(float previousValue, float newValue)
    {

    }

    private void Update()
    {
        if (!IsOwner) { return; }

        if (IsClient)
        {
            ClientUpdate();
        }
        if (IsServer)
        {

        }
    }
    private void ClientUpdate()
    {
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.z = Input.GetAxis("Vertical");
        if (IsMoveInput())
        {
            isSendZeroInput = false;
            RequestMoveServerRpc(moveInput);
        }
        if (IsAttackInput())
        {
            RequestAttackServerRpc();
        }
        SyncMoveAnimationServerRpc(moveInput.sqrMagnitude);

        if (transform.position.y != 0)
        {
            position = transform.position;
            position.y = 0;
            transform.position = position;
        }
    }


    private bool IsMoveInput()
    {
        return Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow);
    }
    private bool IsAttackInput()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }
    [ServerRpc]
    private void RequestAttackServerRpc(ServerRpcParams serverRpcParams = default)
    {
        Debug.Log($"client: {serverRpcParams.Receive.SenderClientId} {NetworkObject.NetworkObjectId} Attack");
        // characterAnimator.SetTrigger("one_hand_right_attack_1");
        networkAnimator.SetTrigger("one_hand_right_attack_1");
    }
    [ServerRpc(Delivery = RpcDelivery.Unreliable)]
    private void SyncMoveAnimationServerRpc(float value)
    {
        characterAnimator.SetFloat("move", value);
    }
    [ServerRpc(Delivery = RpcDelivery.Unreliable)]
    private void RequestMoveServerRpc(Vector3 moveInput)
    {
        characterController.Move(moveInput * moveSpeed.Value * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, Mathf.Atan2(moveInput.x, moveInput.z) * Mathf.Rad2Deg, 0), rotateSpeed.Value * Time.deltaTime);
    }

    public void ServerTakeDamage(float amount)
    {
        if (!IsServer) { return; }
        currentHp.Value = Mathf.Max(0, currentHp.Value - amount);
        if (currentHp.Value == 0)
        {
            NetworkSpawnController.Instance.ServerDeleteCharacter(this);
        }
    }
}
