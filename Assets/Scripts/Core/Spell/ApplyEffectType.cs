[System.Flags]
public enum ApplyEffectType
{
    None = 0,
    Damage = 1 << 1,
    Heal = 1 << 2,
    Custom = 1 << 3
}