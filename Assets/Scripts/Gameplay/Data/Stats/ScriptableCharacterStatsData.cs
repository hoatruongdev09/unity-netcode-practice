using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStatsData", menuName = "Data/ScriptableCharacterStatsData", order = 0)]
public class ScriptableCharacterStatsData : ScriptableObject, ICharacterStatsData
{
    public ICharacterStatParameters[] Stats => stats;

    [SerializeField] private CharacterStatParameters[] stats;
}