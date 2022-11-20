using UnityEngine;

[CreateAssetMenu(fileName = "SphereColliderCreator", menuName = "Data/Rocket/SphereColliderCreator", order = 0)]
public class ScriptableSphereColliderCreator : ARocketColliderCreator
{
    public override Collider CreateCollider(IRocketWorker worker, InGameObject gameObject)
    {
        var collider = gameObject.gameObject.AddComponent<SphereCollider>();
        collider.isTrigger = true;
        collider.radius = worker.Radius;
        collider.center = worker.ModelOffset;
        var rigBody = gameObject.gameObject.AddComponent<Rigidbody>();
        rigBody.useGravity = false;
        // rigBody.isKinematic = true;

        return collider;
    }
}