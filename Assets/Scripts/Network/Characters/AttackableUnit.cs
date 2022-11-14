using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class AttackableUnit : InGameObject
{
    public bool IsDead { get; protected set; }
    public CharacterStats Stats { get; protected set; }
    public IEquipmentManager EquipmentManager { get => equipmentManager; }
    public ICharacterData CharacterData { get; protected set; }
    public ISpell NormalAttackSpell { get; protected set; }
    public Dictionary<int, ISpell> Spells { get; protected set; }
    public ISpell CurrentSpell { get; set; }

    [SerializeField] private BaseEquipmentManager equipmentManager;
    public virtual void InitializeStats(ICharacterStatsData parameters)
    {
        Stats = new CharacterStats(parameters);
    }
    public virtual void InitializeSpells(ISpell[] spells)
    {
        Spells = new Dictionary<int, ISpell>();
        foreach (var spell in spells)
        {
            AddSpell(spell);
        }
    }
    public virtual void LoadCharacterData(ICharacterData characterData)
    {
        CharacterData = characterData;
    }

    protected override void ServerUpdate()
    {
        base.ServerUpdate();
        foreach (var spell in Spells.Values)
        {
            spell?.OnUpdate(Time.deltaTime);
        }
        Stats?.OnUpdate(Time.deltaTime);
    }

    public void TakeDamage(IDamageInfo damageInfo)
    {
        if (!IsServer) { return; }
        var damageTaken = damageInfo.Damage - Stats.GetStat(StatType.Armor).Total;
        Stats.SetCurrentHp(Stats.CurrentHP - damageTaken);
        TakeDamageClientRpc(damageTaken, Stats.CurrentHP);
    }

    [ClientRpc(Delivery = RpcDelivery.Reliable)]
    private void TakeDamageClientRpc(float damage, float currentHp)
    {
        if (!IsOwner) { return; }
        Debug.Log($"clientRpc obj {NetworkObjectId} take damage: {damage} {currentHp}");
    }

    public override void Move(Vector3 direct)
    {
        Move(direct, Stats.GetStat(StatType.MoveSpeed).Total);
    }
    public override void Sprint(Vector3 direct)
    {
        Sprint(direct, Stats.GetStat(StatType.MoveSpeed).Total * 1.4f);
    }
    public override void RotateTo(float x, float y, float z)
    {
        RotateTo(x, y, z, Stats.GetStat(StatType.TurnRate).Total);
    }
    public virtual bool CanCast(ISpell spell)
    {
        if (spell.CurrentState != SpellState.Ready && spell.CurrentState != SpellState.WaitForCast) { return false; }
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
            Spells[slot] = spell;
            return;
        }
        spell.SetOwner(this);
        Spells.Add(slot, spell);
    }
    public virtual void AddSpell(ISpell spell)
    {
        var spellCount = Spells.Count;
        spell.SetOwner(this);
        Spells.Add(spellCount, spell);
    }

    public virtual void EquipItem(int id)
    {
        EquipmentManager.RightHandEquipment = id;
    }
}
