using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class BaseCharacterMovementController : NetworkBehaviour
{
    public virtual void MoveDirect(Vector3 direct, float speed)
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

    }
    public virtual void StartMovement()
    {

    }
}
