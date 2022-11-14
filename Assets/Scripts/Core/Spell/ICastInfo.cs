using UnityEngine;

public interface ICastInfo
{
    Vector3 Location { get; }
    Vector3 Direction { get; }
    ISpell Spell { get; }
    AttackableUnit Target { get; }
    AttackableUnit Owner { get; }
}