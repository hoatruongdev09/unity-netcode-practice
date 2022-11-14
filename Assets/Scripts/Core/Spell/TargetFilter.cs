[System.Flags]
public enum TargetFilter
{
    None = 0,
    Self = 1 << 0,
    Ally = 1 << 1,
    Enemy = 1 << 2,
    Neutral = 1 << 3,
    Dead = 1 << 4
}