using UnityEngine;
[System.Serializable]
public class CharacterStatParameters : ICharacterStatParameters
{
    public IStatData StatData => statData;
    public float BaseValue => baseValue;
    [SerializeField] private ScriptableStatData statData;

    [SerializeField] private float baseValue;
}