using UnityEngine;

public class RocketMovementController : BaseMovementController
{
    public override void MoveDirect(Vector3 direct, float speed, bool force = false)
    {
        transform.Translate(direct * speed * Time.deltaTime, Space.World);
    }
}