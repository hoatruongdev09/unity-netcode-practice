using UnityEngine;

[CreateAssetMenu(fileName = "ConeAoeWorker", menuName = "Data/Spell/Worker/ConeAoeWorker", order = 0)]
public class ConeAoeWorker : ASpellAoeWorker
{
    [SerializeField] private float radius;
    [SerializeField] private float angle;


    public override AttackableUnit[] GetAffectedUnits(Vector3 location, Vector3 direction, AttackableUnit sourceUnit, TargetFilter filter)
    {
        TargetFilterParameters parameters = new TargetFilterParameters()
        {
            Angle = angle,
            Range = radius
        };
        parameters.Position = location;
        parameters.Direct = direction;
        parameters.SourceUnit = sourceUnit;
        parameters.Filter = filter;
        return NetworkSpawnController.Instance.FindNearByUnit(parameters);
    }
}