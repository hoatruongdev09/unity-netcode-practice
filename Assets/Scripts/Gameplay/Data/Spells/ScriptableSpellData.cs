using UnityEngine;

[CreateAssetMenu(fileName = "SpellData", menuName = "Data/SpellData", order = 0)]
public class ScriptableSpellData : ScriptableObject, ISpellData
{
    [field: SerializeField] public int ID { get; set; }

    [field: SerializeField] public string Name { get; set; }

    [field: SerializeField] public Sprite Icon { get; set; }

    [field: SerializeField] public float Cooldown { get; set; }

    [field: SerializeField] public float CastRange { get; set; }

    [field: SerializeField] public TargetFilter TargetFilter { get; set; }

    [field: SerializeField] public CastingType CastingType { get; set; }

    [field: SerializeField] public ApplyEffectType ApplyEffectType { get; set; }

    [field: SerializeField] public ExecuteLogicType ExecuteLogicType { get; set; }

    [field: SerializeField] public bool AutoFindTarget { get; set; }

    [field: SerializeField] public bool UseForwardAsDefault { get; set; }

    [field: SerializeField] public bool UseActorPositionAsDefault { get; set; }

    [field: SerializeField] public bool UseAttackCastPoint { get; set; }

    [field: SerializeField] public float DamageBonus { get; set; }

    [field: SerializeField] public bool HasChannelPhase { get; set; }

    [field: SerializeField] public bool CastImmediatelyAfterChannel { get; set; }

    [field: SerializeField] public bool CastAfterChannel { get; set; }

    [field: SerializeField] public bool CastPreventMove { get; set; }

    [field: SerializeField] public bool CastPreventRotate { get; set; }

    [field: SerializeField] public bool ChannelPreventMove { get; set; }

    [field: SerializeField] public bool ChannelPreventRotate { get; set; }

    [field: SerializeField] public string[] CastAnimation { get; set; }

    [field: SerializeField] public AnimationsPlayMode CastAnimationPlayMode { get; set; }

    [field: SerializeField] public string ChannelAnimation { get; set; }

    [field: SerializeField] public string DashAnimation { get; set; }

    [field: SerializeField] public float AnimationCastPoint { get; set; }

    [field: SerializeField] public float AnimationCastLockPoint { get; set; }

    [field: SerializeField] public float AnimationCancelPoint { get; set; }

    [field: SerializeField] public float MinChannelTime { get; set; }

    [field: SerializeField] public float ChannelDuration { get; set; }

    public ISpellAoeWorker SpellAoeWorker => spellAoeWorker;

    [field: SerializeField] public ASpellAoeWorker spellAoeWorker;

}