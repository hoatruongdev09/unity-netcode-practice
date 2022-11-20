using UnityEngine;
using Unity.Netcode;
public interface IStatData
{
    string Name { get; }
    StatType Type { get; }
    Sprite Icon { get; }
    float MinValue { get; }
    float MaxValue { get; }
    bool UseRoundToInt { get; }
    IStatFormatter Formatter { get; }
}