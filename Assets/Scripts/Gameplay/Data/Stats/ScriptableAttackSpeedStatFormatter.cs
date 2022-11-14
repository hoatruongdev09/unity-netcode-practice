using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableAttackSpeedStatFormatter", menuName = "Data/Utils/ScriptableAttackSpeedStatFormatter", order = 0)]
public class ScriptableAttackSpeedStatFormatter : AStatFormatter
{
    public override float Format(float value)
    {
        return value / 1000;
    }
}