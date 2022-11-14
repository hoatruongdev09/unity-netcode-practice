using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }
    [SerializeField] private float smoothSpeed = 10;
    [SerializeField] private Transform target;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
    }
    private void OnDestroy()
    {
        Instance = null;
    }
    private void LateUpdate()
    {
        if (target == null) { return; }
        transform.position = Vector3.Lerp(transform.position, target.position, smoothSpeed * Time.deltaTime);
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
