using UnityEngine;

public interface IRocketColliderCreator
{
    Collider CreateCollider(IRocketWorker worker, InGameObject gameObject);
}