using UnityEngine;

[CreateAssetMenu(fileName = "NormalStatFormatter", menuName = "Data/Utils/ScriptableNormalStatFormatter", order = 0)]
public class ScriptableNormalStatFormatter : AStatFormatter
{
    public override float Format(float value)
    {
        return value;
    }
}