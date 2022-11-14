using UnityEngine;
public abstract class AStatFormatter : ScriptableObject, IStatFormatter
{
    public abstract float Format(float value);
}