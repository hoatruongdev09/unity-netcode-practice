using UnityEngine;

public interface ISpellAoeWorker
{
    AttackableUnit[] GetAffectedUnits(Vector3 localtion, Vector3 direction, AttackableUnit sourceUnit, TargetFilter filter);
}