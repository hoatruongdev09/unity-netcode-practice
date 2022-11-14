using UnityEngine;

public class TargetFilterParameters
{
    public AttackableUnit SourceUnit { get; set; }
    public TargetFilter Filter { get; set; }
    public float Range { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 Direct { get; set; }
    public float Angle { get; set; }
}