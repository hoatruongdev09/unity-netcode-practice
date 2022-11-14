using UnityEngine;

public abstract class ASpellAoeWorker : ScriptableObject, ISpellAoeWorker
{
    public abstract AttackableUnit[] GetAffectedUnits(Vector3 localtion, Vector3 direction, AttackableUnit sourceUnit, TargetFilter filter);
}