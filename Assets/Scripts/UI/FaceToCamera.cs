using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceToCamera : MonoBehaviour
{
    [SerializeField] private Canvas canvas;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        canvas.worldCamera = mainCamera;
    }

    private void LateUpdate()
    {
        transform.rotation = mainCamera.transform.rotation;
    }
}
