using UnityEngine;

public interface ISpellData
{
    int ID { get; }
    string Name { get; }
    Sprite Icon { get; }
    float Cooldown { get; }
    float CastRange { get; }
    TargetFilter TargetFilter { get; }
    CastingType CastingType { get; }
    ApplyEffectType ApplyEffectType { get; }
    ExecuteLogicType ExecuteLogicType { get; }
    SpellFinishType SpellFinishType { get; }
    bool HasChannelPhase { get; }
    bool CastImmediatelyAfterChannel { get; }
    bool CastAfterChannel { get; }
    bool AutoFindTarget { get; }
    bool UseForwardAsDefault { get; }
    bool UseActorPositionAsDefault { get; }
    bool UseAttackCastPoint { get; }
    float DamageBonus { get; }

    bool CastPreventMove { get; }
    bool CastPreventRotate { get; }
    bool ChannelPreventMove { get; }
    bool ChannelPreventRotate { get; }
    bool CanCancelWhileCasting { get; }
    bool CanCancelWhileChanneling { get; }
    bool CanCancelWhileDashing { get; }

    string[] CastAnimation { get; }
    AnimationsPlayMode CastAnimationPlayMode { get; }
    string ChannelAnimation { get; }
    string DashAnimation { get; }

    float AnimationCastLockPoint { get; }
    float AnimationCastPoint { get; }
    float AnimationCancelPoint { get; }
    float MinChannelTime { get; }
    float ChannelDuration { get; }

    ISpellAoeWorker SpellAoeWorker { get; }
    IRocketWorker RocketWorker { get; }
    IDashWorker DashWorker { get; }
}