using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class AttackableUnit : InGameObject
{
    public bool IsDead { get; protected set; }
    public ICharacterStats Stats { get => characterStats; }
    public IEquipmentManager EquipmentManager { get => equipmentManager; }
    public ICharacterData CharacterData { get; protected set; }
    public ISpell NormalAttackSpell { get; protected set; }
    public Dictionary<int, ISpell> Spells { get; protected set; }
    public ISpell CurrentSpell { get; set; }

    [SerializeField] private NetworkCharacterStats characterStats;
    [SerializeField] private BaseEquipmentManager equipmentManager;
    [SerializeField] private CharacterHeadUI characterHeadUI;

    public virtual void InitializeStats(ICharacterStatsData parameters)
    {
        if (IsServer)
        {
            characterStats.InitializeStats(parameters);
        }
    }
    public virtual void InitializeSpells(ISpell[] spells)
    {
        if (IsServer)
        {
            Spells = new Dictionary<int, ISpell>();
            foreach (var spell in spells)
            {
                AddSpell(spell);
            }
        }
    }
    public virtual void LoadCharacterData(ICharacterData characterData)
    {
        CharacterData = characterData;
    }

    protected override void ServerUpdate()
    {
        base.ServerUpdate();
        if (Spells != null)
        {
            foreach (var spell in Spells.Values)
            {
                spell?.OnUpdate(Time.deltaTime);
            }
        }
        Stats?.OnUpdate(Time.deltaTime);
    }


    public void TakeDamage(IDamageInfo damageInfo)
    {
        if (!IsServer) { return; }
        float lastHp = Stats.CurrentHP;
        float damageTaken = damageInfo.Damage - Stats.GetStat(StatType.Armor).Total;
        Stats.SetCurrentHp(Stats.CurrentHP - damageTaken);
        TakeDamageClientRpc(damageTaken, Stats.CurrentHP / Stats.GetStat(StatType.HealthPoint).Total, lastHp / Stats.GetStat(StatType.HealthPoint).Total);
        if (Stats.CurrentHP <= 0)
        {
            Die(damageInfo.From);
        }
    }

    private void Die(AttackableUnit killer)
    {
        SetDied();
        transform.LookAt(killer.Position, Vector3.up);
        DisableColliders();
        PlayAnimation("death");
        GameHelper.TriggerCharacterDieSignal(killer, this);
    }

    [ClientRpc(Delivery = RpcDelivery.Reliable)]
    private void TakeDamageClientRpc(float damage, float currentHpPercent, float lastHpPercent)
    {
        Debug.Log($"clientRpc obj {NetworkObjectId} take damage: {damage} {lastHpPercent} {currentHpPercent}");
        characterHeadUI.SetHeadHP(lastHpPercent, currentHpPercent);
    }

    public override void SetDied()
    {
        IsDead = true;
        isDied.Value = true;
    }
    public override bool IsAlive()
    {
        return base.IsAlive() && !IsDead;
    }
    public override void Move(Vector3 direct)
    {
        Move(direct, Stats.GetStat(StatType.MoveSpeed).Total);
    }
    public override void Sprint(Vector3 direct)
    {
        Sprint(direct, Stats.GetStat(StatType.MoveSpeed).Total * 1.9f);
    }

    public override void RotateTo(float x, float y, float z)
    {
        RotateTo(x, y, z, Stats.GetStat(StatType.TurnRate).Total);
    }
    public virtual bool CanCast(ISpell spell)
    {
        if (spell.CurrentState != SpellState.Ready && spell.CurrentState != SpellState.WaitForCast) { return false; }
        if (CurrentSpell != null)
        {
            if (CurrentSpell.CurrentState == SpellState.Casting && !CurrentSpell.SpellData.CanCancelWhileCasting)
            {
                return false;
            }
            if (CurrentSpell.CurrentState == SpellState.Channeling && !CurrentSpell.SpellData.CanCancelWhileChanneling)
            {
                return false;
            }
            if (CurrentSpell.CurrentState == SpellState.Dashing && !CurrentSpell.SpellData.CanCancelWhileDashing)
            {
                return false;
            }
            CurrentSpell.Interrupt();
        }
        return true;
    }
    public virtual void CastSpell(ISpell spell, AttackableUnit target, Vector3 direct, Vector3 location)
    {
        spell.Execute(target, location, direct);
    }
    public virtual ISpell GetSpell(int slot)
    {
        if (Spells.TryGetValue(slot, out var spell))
        {
            return spell;
        }
        else
        {
            return null;
        }
    }
    public virtual ISpell GetSellByID(int id)
    {
        foreach (var spell in Spells.Values)
        {
            if (spell.SpellData.ID == id) { return spell; }
        }
        return null;
    }
    public virtual void AddSpell(int slot, ISpell spell, bool allowReplace = true)
    {
        if (Spells.ContainsKey(slot) && allowReplace)
        {
            Spells[slot].SetOwner(null);
            spell.SetOwner(this);
            spell.Active();
            Spells[slot] = spell;
            return;
        }
        spell.SetOwner(this);
        Spells.Add(slot, spell);
        spell.Active();
    }
    public virtual void AddSpell(ISpell spell)
    {
        var spellCount = Spells.Count;
        spell.SetOwner(this);
        Spells.Add(spellCount, spell);
        spell.Active();
    }
    public virtual void RemoveSpell(ISpell spell)
    {
        if (!Spells.ContainsValue(spell)) { return; }
        spell.Disable();
        spell.SetOwner(null);
        foreach (var pair in Spells)
        {
            if (pair.Value == spell)
            {
                Spells.Remove(pair.Key);
                break;
            }
        }
    }

    public virtual void EquipItem(int id)
    {
        EquipmentManager.RightHandEquipment = id;
    }
}
