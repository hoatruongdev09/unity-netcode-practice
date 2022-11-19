using deVoid.Utils;
using UnityEngine;

public class Spell : ISpell
{

    public ISpellData SpellData { get; private set; }

    public AttackableUnit Owner { get; private set; }

    public SpellState CurrentState { get; private set; }

    public ICastInfo CurrentCastInfo { get; private set; }

    public float CurrentChannelTime { get; private set; }

    public float CurrentCastTime { get; private set; }

    public float CurrentCooldown { get; private set; }

    private float lastCalculatedCastTime;
    private float lastCalculatedLockPoint;
    private float lastCalculatedCancelTime;

    private float lastCalculatedMinChannelTime;
    private float lastCalculatedMaxChannelTime;

    private bool casted;
    private bool isCastLocked;
    private int castAnimationPlayIndex;


    public Spell(ISpellData spellData, AttackableUnit owner)
    {
        SpellData = spellData;
        Owner = owner;
        SwitchState(SpellState.Ready);
    }

    public void Active()
    {
        Signals.Get<RocketTriggerObject>().AddListener(OnRocketTriggerObject);
    }

    public void Disable()
    {
        Signals.Get<RocketTriggerObject>().RemoveListener(OnRocketTriggerObject);
    }

    public bool Execute(AttackableUnit target, Vector3 castLocation, Vector3 castDirection)
    {
        if (CurrentState == SpellState.Cooldown) { return false; }
        Vector3 direct = castDirection;
        Vector3 location = castLocation;
        if (target != null && SpellData.CastingType.HasFlag(CastingType.Target) && GameHelper.GetSqrDistance(target.Position, GetActorPosition()) > Mathf.Pow(SpellData.CastRange, 2))
        {
            return false;
        }
        AttackableUnit nearestTarget = FindNearestTarget(GetActorPosition(), SpellData.CastRange, SpellData.TargetFilter, Vector3.zero, 360f);
        if (target == null && SpellData.CastingType.HasFlag(CastingType.Target))
        {
            if (!SpellData.AutoFindTarget) { return false; }
            target = nearestTarget;
            if (target == null && SpellData.CastingType.HasFlag(CastingType.Self))
            {
                target = GetActor();
            }
            if (target == null) { return false; }
        }
        if (target == null && (SpellData.CastingType & (CastingType.Self | CastingType.SelfDirection)) != 0)
        {
            target = GetActor();
        }
        if (direct.sqrMagnitude == 0 && SpellData.CastingType.HasFlag(CastingType.SelfDirection))
        {
            if (SpellData.AutoFindTarget && !SpellData.UseForwardAsDefault)
            {
                direct = (nearestTarget.Position - GetActorPosition()).normalized;
            }
            if (direct.sqrMagnitude == 0 && SpellData.UseForwardAsDefault)
            {
                direct = GetActor().Forward;
            }
            if (direct.sqrMagnitude == 0) { return false; }
        }
        if (castLocation == GameHelper.UNSET_VECTOR_3 && SpellData.CastingType.HasFlag(CastingType.AOE))
        {
            if (SpellData.AutoFindTarget && !SpellData.UseActorPositionAsDefault)
            {
                location = nearestTarget.Position;
            }
            if (location == GameHelper.UNSET_VECTOR_3 && SpellData.UseActorPositionAsDefault)
            {
                location = GetActorPosition();
            }
            if (location == GameHelper.UNSET_VECTOR_3) { return false; }

        }
        casted = false;
        isCastLocked = false;
        Execute(new CastInfo(location, direct, target, GetActor(), this));
        return true;
    }
    public void Execute(ICastInfo castInfo)
    {
        CurrentCastInfo = castInfo;
        Debug.Log($"Excute: {castInfo.Target == null} ({castInfo.Location.x},{castInfo.Location.y},{castInfo.Location.z}) ({castInfo.Direction.x},{castInfo.Direction.y},{castInfo.Direction.z})");

        if (SpellData.HasChannelPhase && CurrentState == SpellState.Ready)
        {
            StartChannel();
            return;
        }
        if (SpellData.HasChannelPhase && CurrentState == SpellState.Channeling && CurrentChannelTime >= lastCalculatedMinChannelTime)
        {
            FinishChanel();
            return;
        }
        else
        {
            StartCast();
        }
    }
    public AttackableUnit FindNearestTarget(Vector3 position, float range, TargetFilter filter, Vector3 direct, float angle)
    {
        return NetworkSpawnController.Instance.FindNearestUnit(new TargetFilterParameters()
        {
            SourceUnit = GetActor(),
            Position = position,
            Filter = filter,
            Range = GetCastRange(),
            Direct = direct,
            Angle = angle
        });
    }

    public void OnPreCast()
    {

    }

    public void OnSpellCast()
    {
        casted = true;
        if (SpellData.ExecuteLogicType.HasFlag(ExecuteLogicType.Aoe))
        {
            ApplyAoe(CurrentCastInfo);
        }
        if (SpellData.ExecuteLogicType.HasFlag(ExecuteLogicType.Rocket))
        {
            ApplyRocket(CurrentCastInfo);
        }
        if (SpellData.ExecuteLogicType.HasFlag(ExecuteLogicType.Dash))
        {
            ApplyDash(CurrentCastInfo);
        }
    }

    public void ApplyEffect(AttackableUnit unit)
    {
        switch (SpellData.ApplyEffectType)
        {
            case ApplyEffectType.Damage:
                var damageInfo = new DamageInfo(GameHelper.GeneratePacketId(), GetActor(), unit, SpellData.DamageBonus);
                Debug.Log($"{unit.NetworkObjectId} take damage");
                GameHelper.TriggerDamageSignal(damageInfo);
                break;
            case ApplyEffectType.Heal:
                break;
            case ApplyEffectType.Custom:
                break;
        }
    }

    private void ApplyRocket(ICastInfo currentCastInfo)
    {
        var direct = currentCastInfo.Direction;
        var target = currentCastInfo.Target;
        var owner = currentCastInfo.Owner;
        var gameObject = NetworkSpawnController.Instance.CreateRocket(owner.Position, owner.Rotation, currentCastInfo, direct, target, SpellData.RocketWorker);
        var rocket = gameObject.GetComponent<Rocket>();
        rocket.NetworkOwnerID = Owner.NetworkOwnerID;
    }

    private void ApplyAoe(ICastInfo currentCastInfo)
    {
        var units = SpellData.SpellAoeWorker.GetAffectedUnits(currentCastInfo.Location, currentCastInfo.Direction, currentCastInfo.Owner, SpellData.TargetFilter);
        foreach (var unit in units)
        {
            ApplyEffect(unit);
        }
    }
    private void ApplyDash(ICastInfo currentCastInfo)
    {
        var direct = currentCastInfo.Direction;
        var target = currentCastInfo.Target;
        SpellData.DashWorker.DashLogic.Move(target, SpellData.DashWorker, direct, OnDashStart, OnDashFinish);
    }

    private void OnDashStart(InGameObject target)
    {
        target.SetActiveMovement(false);
        target.SetActiveRotation(false);
        target.PlayAnimation(SpellData.DashAnimation, 1);
    }

    private void OnDashFinish(InGameObject target)
    {
        target.SetActiveMovement(true);
        target.SetActiveRotation(true);
    }

    private void OnRocketTriggerObject(Rocket rocket, InGameObject obj)
    {
        if (rocket.CastInfo == null || rocket.CastInfo.Spell != this) { return; }
        var attackableUnit = obj as AttackableUnit;

        if (attackableUnit)
        {
            RocketTriggerCharacter(rocket, attackableUnit);
        }

    }
    private void RocketTriggerCharacter(Rocket rocket, AttackableUnit character)
    {
        if (!rocket.IsAlive()) { return; }
        if (!SpellHelper.IsValidTarget(Owner, character, SpellData.TargetFilter)) { return; }
        if (rocket.CurrentCharacterHitTime >= rocket.RocketWorker.LimitCharacterTime)
        {
            NetworkSpawnController.Instance.RemoveGameObject(rocket.NetworkObjectId);
            return;
        }
        ApplyEffect(character);
        rocket.SetCurrentHitTime(rocket.CurrentCharacterHitTime + 1);
    }

    public void OnPostCast()
    {
        casted = true;
        if (HasCastAnimation(castAnimationPlayIndex))
        {
            Owner.SetAnimation("cast_or_channel_spell", false);
        }
        if (SpellData.CastPreventMove)
        {
            Owner.SetActiveMovement(true);
        }
        if (SpellData.CastPreventRotate)
        {
            Owner.SetActiveRotation(true);
        }
    }
    protected virtual bool HasCastAnimation(int index)
    {
        return SpellData.CastAnimation.Length != 0 && !string.IsNullOrEmpty(SpellData.CastAnimation[index]);
    }
    public void SwitchState(SpellState nextState)
    {
        CurrentState = nextState;
    }
    public void OnUpdate(float delta)
    {
        switch (CurrentState)
        {
            case SpellState.Ready:
                break;
            case SpellState.WaitForCast:
                break;
            case SpellState.Channeling:
                OnChanneling(delta);
                break;
            case SpellState.Casting:
                OnCasting(delta);
                break;
            case SpellState.Cooldown:
                CurrentCooldown = Mathf.Max(0, CurrentCooldown - delta);
                if (CurrentCooldown == 0)
                {
                    SwitchState(SpellState.Ready);
                }
                break;
        }
    }

    public void SetOwner(AttackableUnit owner)
    {
        Owner = owner;
    }

    public void StartCast()
    {
        if (SpellData.CastPreventMove)
        {
            Owner.SetActiveMovement(false);
        }

        if (SpellData.CastPreventRotate)
        {
            Owner.SetActiveRotation(false);
        }
        lastCalculatedLockPoint = GetAnimationLockPoint();
        lastCalculatedCancelTime = GetAnimationCancelTime();
        lastCalculatedCastTime = GetAnimationCastTime();
        CurrentCastTime = 0;
        SwitchState(SpellState.Casting);
        float animationPlaySpeed = SpellData.UseAttackCastPoint ? Owner.Stats.GetStat(StatType.AttackSpeed).Total : 1;
        PlayAnimation(Owner, SpellData.CastAnimation, animationPlaySpeed, SpellData.CastAnimationPlayMode, ref castAnimationPlayIndex);
        if (HasCastAnimation(castAnimationPlayIndex))
        {
            Owner.SetAnimation("cast_or_channel_spell", true);
        }
        OnPreCast();
    }



    public void StartChannel()
    {
        if (SpellData.ChannelPreventMove)
        {
            Owner.SetActiveMovement(false);
        }
        if (SpellData.ChannelPreventRotate)
        {
            Owner.SetActiveRotation(false);
        }
        lastCalculatedMinChannelTime = GetMinChannelDuration();
        lastCalculatedMaxChannelTime = GetMaxChannelDuration();
        CurrentChannelTime = 0;
        Owner.PlayAnimation(SpellData.ChannelAnimation);
        if (!string.IsNullOrEmpty(SpellData.ChannelAnimation))
        {
            Owner.SetAnimation("cast_or_channel_spell", true);
        }
        SwitchState(SpellState.Channeling);
    }

    public void StartCooldown()
    {
        Debug.Log("start cooldown");
        CurrentCooldown = SpellData.Cooldown;
        SwitchState(SpellState.Cooldown);
    }

    public void OnChanneling(float delta)
    {
        CurrentChannelTime = Mathf.Min(lastCalculatedMaxChannelTime, CurrentChannelTime + delta);
        if (CurrentChannelTime == lastCalculatedMaxChannelTime)
        {
            FinishChanel();
        }

    }
    public void FinishChanel()
    {
        if (!string.IsNullOrEmpty(SpellData.ChannelAnimation))
        {
            Owner.SetAnimation("cast_or_channel_spell", false);
        }
        if (SpellData.ChannelPreventMove)
        {
            Owner.SetActiveMovement(true);
        }
        if (SpellData.ChannelPreventRotate)
        {
            Owner.SetActiveRotation(true);
        }
        if (SpellData.CastImmediatelyAfterChannel)
        {
            StartCast();
        }
        else if (SpellData.CastAfterChannel)
        {
            SwitchState(SpellState.WaitForCast);
        }
        else
        {
            StartCooldown();
        }
    }
    public void OnCasting(float delta)
    {
        CurrentCastTime = Mathf.Min(lastCalculatedCancelTime, CurrentCastTime + delta);
        if (CurrentCastTime > lastCalculatedLockPoint)
        {
            OnSpellCastLock();
        }
        if (CurrentCastTime >= lastCalculatedCastTime && !casted)
        {
            OnSpellCast();
        }
        if (CurrentCastTime == lastCalculatedCancelTime)
        {
            OnPostCast();
            StartCooldown();
            return;
        }
    }

    private void OnSpellCastLock()
    {
        if (isCastLocked) { return; }
        isCastLocked = true;

    }

    public float GetAnimationCastTime()
    {
        float castPoint = SpellData.AnimationCastPoint;
        if (SpellData.UseAttackCastPoint)
        {
            castPoint /= Owner.Stats.GetStat(StatType.AttackSpeed).Total;
        }
        return castPoint;
    }

    private float GetAnimationLockPoint()
    {
        float lockPoint = SpellData.AnimationCastLockPoint;
        if (SpellData.UseAttackCastPoint)
        {
            lockPoint /= Owner.Stats.GetStat(StatType.AttackSpeed).Total;
        }
        return lockPoint;
    }
    public float GetAnimationCancelTime()
    {
        float castCancelPoint = SpellData.AnimationCancelPoint;
        if (SpellData.UseAttackCastPoint)
        {
            castCancelPoint /= Owner.Stats.GetStat(StatType.AttackSpeed).Total;
        }
        return castCancelPoint;
    }

    public float GetMinChannelDuration()
    {
        return SpellData.MinChannelTime;
    }

    public float GetMaxChannelDuration()
    {
        return SpellData.ChannelDuration;
    }

    public float GetCastRange()
    {
        return SpellData.CastRange;
    }

    public Vector3 GetActorPosition()
    {
        return Owner.Position;
    }
    public AttackableUnit GetActor()
    {
        return Owner;
    }

    public void PlayAnimation(InGameObject controller, string[] animations, float speed, AnimationsPlayMode mode, ref int index)
    {
        if (animations.Length == 0) { return; }
        switch (mode)
        {
            case AnimationsPlayMode.RoundRobin:
                index++;
                index = index >= animations.Length ? 0 : index;
                controller.PlayAnimation(animations[index], speed);
                break;
            case AnimationsPlayMode.Random:
                index = Random.Range(0, animations.Length);
                controller.PlayAnimation(animations[index], speed);
                break;
            case AnimationsPlayMode.Fixed:
                controller.PlayAnimation(animations[index], speed);
                break;
            default: break;
        }
    }


}