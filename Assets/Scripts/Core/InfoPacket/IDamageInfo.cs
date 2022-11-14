public interface IDamageInfo : IPacketInfo
{
    AttackableUnit From { get; }
    AttackableUnit Target { get; }
    float Damage { get; }
}