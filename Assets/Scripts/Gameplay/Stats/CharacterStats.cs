using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class CharacterStats : ICharacterStats
{
    [field: SerializeField] public float CurrentHP { get; private set; }
    [field: SerializeField] public float CurrentMana { get; private set; }

    private Dictionary<StatType, IStat> stats = new Dictionary<StatType, IStat>();

    private IStat hpStat;
    private IStat manaStat;
    private IStat hpRegenStat;
    private IStat manaRegenStat;

    private bool initialized;

    public CharacterStats(ICharacterStatsData parameters)
    {
        foreach (var stat in parameters.Stats)
        {
            AddStat(stat.StatData, stat.BaseValue);
        }

        hpStat = GetStat(StatType.HealthPoint);
        hpRegenStat = GetStat(StatType.HpRegen);
        manaStat = GetStat(StatType.Mana);
        manaRegenStat = GetStat(StatType.ManaRegen);

        SetCurrentHp(hpStat.Total);
        SetCurrentMana(manaStat.Total);
        initialized = true;
    }

    public void OnUpdate(float delta)
    {
        if (!initialized) { return; }
        if (CurrentHP > 0 && CurrentHP < hpStat.Total && hpRegenStat.Total > 0)
        {
            CurrentHP = Mathf.Min(CurrentHP + delta * hpRegenStat.Total, hpStat.Total);
        }
        if (CurrentMana > 0 && CurrentMana < manaStat.Total && manaRegenStat.Total > 0)
        {
            CurrentMana = Mathf.Min(CurrentMana + delta * manaRegenStat.Total, manaStat.Total);
        }
    }
    public void SetCurrentHp(float value)
    {
        CurrentHP = Mathf.Max(0, Mathf.Min(value, hpStat.Total));
    }

    public IStat[] GetAllStats()
    {
        return stats.Values.ToArray();
    }

    public IStat GetStat(StatType type)
    {
        if (stats.TryGetValue(type, out var stat))
        {
            return stat;
        }
        else { return null; }
    }

    public void AddStat(IStatData statData, float baseValue)
    {
        stats.Add(statData.Type, new Stat(baseValue, statData));
    }

    public void SetCurrentMana(float value)
    {
        CurrentMana = Mathf.Max(0, Mathf.Min(value, manaStat.Total));
    }
}