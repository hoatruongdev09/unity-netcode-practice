using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class BaseMovementController : NetworkBehaviour
{
    protected bool isStopMovement;
    protected bool isDashing;
    public virtual void MoveDirect(Vector3 direct, float speed, bool force = false)
    {

    }
    public virtual void RotateTo(float x, float y, float z)
    {
        transform.rotation = Quaternion.Euler(x, y, z);
    }
    public virtual void RotateTo(float x, float y, float z, float speed)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(x, y, z), speed * Time.deltaTime);
    }
    public virtual void StopMovement()
    {
        isStopMovement = true;
    }
    public virtual void StartMovement()
    {
        isStopMovement = false;
    }
    public virtual void StartDashing()
    {
        isDashing = true;
    }
    public virtual void StopDashing()
    {
        isDashing = false;
    }
}
