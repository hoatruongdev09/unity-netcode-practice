using UnityEngine;

public interface ISpell : IUpdate
{
    ISpellData SpellData { get; }
    AttackableUnit Owner { get; }
    SpellState CurrentState { get; }
    ICastInfo CurrentCastInfo { get; }
    float CurrentChannelTime { get; }
    float CurrentCastTime { get; }
    float CurrentCooldown { get; }

    void Active();
    void Disable();

    void SetOwner(AttackableUnit owner);
    void SwitchState(SpellState nextState);
    AttackableUnit FindNearestTarget(Vector3 position, float range, TargetFilter filter, Vector3 direct, float angle);
    bool Execute(AttackableUnit target, Vector3 castLocation, Vector3 castDirection);
    void Execute(ICastInfo castInfo);
    void OnChanneling(float delta);
    void OnCasting(float delta);


    void StartChannel();
    void StartCast();
    void StartCooldown();

    void OnSpellCast();
    void FinishChanel();

    void OnPreCast();
    void OnPostCast();

    float GetAnimationCastTime();
    float GetAnimationCancelTime();

    float GetMinChannelDuration();
    float GetMaxChannelDuration();

    float GetCastRange();

    Vector3 GetActorPosition();
    AttackableUnit GetActor();

    void PlayAnimation(InGameObject controller, string[] animations, float speed, AnimationsPlayMode mode, ref int index);

}