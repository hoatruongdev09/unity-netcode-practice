using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System.Linq;
using System;

public class NetworkCharacterStats : NetworkBehaviour, ICharacterStats
{
    public float CurrentHP => currentHp?.Value ?? 0;
    public float CurrentMana => currentMana?.Value ?? 0;

    [SerializeField] private CharacterHeadUI characterHeadUI;

    private Dictionary<StatType, IStat> stats = new Dictionary<StatType, IStat>();
    private NetworkVariable<float> currentHp = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<float> currentMana = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private IStat hpStat;
    private IStat manaStat;
    private IStat hpRegenStat;
    private IStat manaRegenStat;

    private bool initialized;

    public void InitializeStats(ICharacterStatsData parameters)
    {
        if (IsServer)
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

    }

    public void AddStat(IStatData statData, float baseValue)
    {
        stats.Add(statData.Type, new Stat(baseValue, statData));
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
        return null;
    }

    public void OnUpdate(float delta)
    {
        if (!initialized) { return; }
        if (CurrentHP > 0 && CurrentHP < hpStat.Total && hpRegenStat.Total > 0)
        {
            SetCurrentHp(Mathf.Min(CurrentHP + delta * hpRegenStat.Total, hpStat.Total));
        }
        if (CurrentMana > 0 && CurrentMana < manaStat.Total && manaRegenStat.Total > 0)
        {
            SetCurrentMana(Mathf.Min(CurrentMana + delta * manaRegenStat.Total, manaStat.Total));
        }
    }

    public void SetCurrentHp(float value)
    {
        currentHp.Value = Mathf.Max(0, Mathf.Min(value, hpStat.Total));
        SetCurrentHpClientRpc(currentHp.Value / hpStat.Total);
    }

    public void SetCurrentMana(float value)
    {
        currentMana.Value = Mathf.Max(0, Mathf.Min(value, manaStat.Total));
        SetCurrentManaClientRpc(currentMana.Value / manaRegenStat.Total);
    }

    [ClientRpc]
    private void SetCurrentManaClientRpc(float manaPercent)
    {

    }

    [ClientRpc]
    private void SetCurrentHpClientRpc(float hpPercent)
    {
        characterHeadUI.SetHeadHP(null, hpPercent);
    }


}
