using UnityEngine;


public abstract class ARocketMoveLogic : ScriptableObject, IRocketMoveLogic
{
    public abstract void Move(Rocket rocket, float deltaTime);
}