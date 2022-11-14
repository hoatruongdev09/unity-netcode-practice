using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Data/CharacterData", order = 0)]
public class ScriptableCharacterData : ScriptableObject, ICharacterData
{
    [field: SerializeField] public int ID { get; set; }
    public ICharacterStatsData StatsData => statsData;

    public ISpellData NormalAttackSpell => normalAttackSpell;

    public ISpellData[] ActiveSpells => activeSpells;

    public ISpellData[] PassiveSpell => passiveSpells;

    [SerializeField] private ScriptableCharacterStatsData statsData;
    [SerializeField] private ScriptableSpellData normalAttackSpell;
    [SerializeField] private ScriptableSpellData[] activeSpells;
    [SerializeField] private ScriptableSpellData[] passiveSpells;

    public ISpellData[] GetAllSpells()
    {
        var spells = new ISpellData[1 + activeSpells.Length + passiveSpells.Length];
        int index = 0;
        spells[index++] = normalAttackSpell;
        for (int i = 0; i < activeSpells.Length; i++)
        {
            spells[index++] = activeSpells[i];
        }
        for (int i = 0; i < passiveSpells.Length; i++)
        {
            spells[index++] = passiveSpells[i];
        }
        return spells;
    }
}