using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCharacterRigidbodyMovementController : BaseMovementController
{
    [SerializeField] private float stopMass = 10000;
    [SerializeField] private float stopDrag = 1000;
    [SerializeField] private Rigidbody rigidbody;


    public override void MoveDirect(Vector3 direct, float speed, bool force = false)
    {
        if (IsDashing && !force) { return; }
        rigidbody.velocity = direct * speed;
        // transform.Translate(direct * speed * Time.deltaTime, Space.World);
    }
    public override void StopMovement()
    {
        rigidbody.velocity = Vector3.zero;
        rigidbody.mass = stopMass;
        rigidbody.drag = stopDrag;
        base.StopMovement();
    }
    public override void StartMovement()
    {
        rigidbody.mass = 1;
        rigidbody.drag = 0;
        base.StartMovement();
    }
    public override void StartDashing()
    {
        rigidbody.mass = 1;
        rigidbody.drag = 0;
        base.StartDashing();
    }
    public override void StopDashing()
    {
        rigidbody.velocity = Vector3.zero;
        rigidbody.mass = stopMass;
        rigidbody.drag = stopDrag;

        base.StopDashing();
    }
}
