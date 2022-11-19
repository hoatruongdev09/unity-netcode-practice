using UnityEngine;

[CreateAssetMenu(fileName = "RocketDirectMoveLogic", menuName = "Data/Rocket/RocketDirectMoveLogic", order = 0)]
public class ScriptableRocketDirectMoveLogic : ARocketMoveLogic
{
    public override void Move(Rocket rocket, float deltaTime)
    {
        rocket.Move(rocket.MoveDirect, rocket.RocketWorker.Speed);
    }

}