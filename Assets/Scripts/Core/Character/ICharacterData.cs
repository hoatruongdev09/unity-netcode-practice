public interface ICharacterData
{
    int ID { get; }
    ICharacterStatsData StatsData { get; }
    ISpellData NormalAttackSpell { get; }
    ISpellData[] ActiveSpells { get; }
    ISpellData[] PassiveSpell { get; }

    ISpellData[] GetAllSpells();
}