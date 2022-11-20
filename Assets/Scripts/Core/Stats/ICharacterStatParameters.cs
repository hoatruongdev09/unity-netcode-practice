using Unity.Netcode;
public interface ICharacterStatParameters
{
    IStatData StatData { get; }
    float BaseValue { get; }
}