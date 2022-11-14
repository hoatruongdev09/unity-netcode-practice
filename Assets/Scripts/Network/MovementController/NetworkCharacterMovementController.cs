using UnityEngine;

public class NetworkCharacterMovementController : BaseCharacterMovementController
{
    [SerializeField] private CharacterController characterController;
    public override void MoveDirect(Vector3 direct, float speed)
    {
        characterController.SimpleMove(direct * speed);
    }
}