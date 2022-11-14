using UnityEngine;

public class CastInfo : ICastInfo
{
    public Vector3 Location { get; set; }

    public Vector3 Direction { get; set; }

    public AttackableUnit Target { get; set; }

    public AttackableUnit Owner { get; set; }

    public ISpell Spell { get; set; }

    public CastInfo(Vector3 location, Vector3 direction, AttackableUnit target, AttackableUnit owner, ISpell spell)
    {
        Location = location;
        Direction = direction;
        Target = target;
        Owner = owner;
        Spell = spell;
    }
}