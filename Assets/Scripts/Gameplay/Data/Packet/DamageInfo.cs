public class DamageInfo : BasePacket, IDamageInfo
{

    public AttackableUnit From { get; set; }

    public AttackableUnit Target { get; set; }

    public float Damage { get; set; }

    public DamageInfo(long packetID, AttackableUnit from, AttackableUnit target, float damage) : base(packetID)
    {
        From = from;
        Target = target;
        Damage = damage;
    }
}