using UnityEngine;

public abstract class ARocketColliderCreator : ScriptableObject, IRocketColliderCreator
{
    public abstract Collider CreateCollider(IRocketWorker worker, InGameObject gameObject);
}