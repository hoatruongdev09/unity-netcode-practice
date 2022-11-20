using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "StatData", menuName = "Data/ScriptableStatData", order = 0)]
public class ScriptableStatData : ScriptableObject, IStatData
{
    [field: SerializeField] public string Name { get; set; }

    [field: SerializeField] public StatType Type { get; set; }

    [field: SerializeField] public Sprite Icon { get; set; }

    [field: SerializeField] public float MinValue { get; set; }

    [field: SerializeField] public float MaxValue { get; set; }

    [field: SerializeField] public bool UseRoundToInt { get; set; }

    public IStatFormatter Formatter => formatter;
    [SerializeField] private AStatFormatter formatter;


}