public interface ICharacterStats : IUpdate
{
    IStat[] GetAllStats();
    float CurrentHP { get; }
    float CurrentMana { get; }

    IStat GetStat(StatType type);
    void AddStat(IStatData statData, float baseValue);
    void SetCurrentHp(float value);
    void SetCurrentMana(float value);
}