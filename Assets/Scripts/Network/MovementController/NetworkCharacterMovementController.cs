using UnityEngine;

public class NetworkCharacterMovementController : BaseMovementController
{
    [SerializeField] private CharacterController characterController;
    public override void MoveDirect(Vector3 direct, float speed, bool force = false)
    {
        characterController.SimpleMove(direct * speed);
    }

}