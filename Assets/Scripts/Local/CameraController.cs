using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public enum UpdateTpe { Update, LateUpdate, FixedUpdate }
    public static CameraController Instance { get; private set; }
    [SerializeField] private float smoothSpeed = 10;
    [SerializeField] private Transform target;
    [SerializeField] private UpdateTpe updateType;

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
        if (updateType != UpdateTpe.LateUpdate) { return; }
        if (target == null) { return; }
        transform.position = Vector3.Lerp(transform.position, target.position, smoothSpeed * Time.deltaTime);
    }
    private void Update()
    {
        if (updateType != UpdateTpe.Update) { return; }
        if (target == null) { return; }
        transform.position = Vector3.Lerp(transform.position, target.position, smoothSpeed * Time.deltaTime);
    }
    private void FixedUpdate()
    {
        if (updateType != UpdateTpe.FixedUpdate) { return; }
        if (target == null) { return; }
        transform.position = Vector3.Lerp(transform.position, target.position, smoothSpeed * Time.deltaTime);
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
