public interface IStat
{
    IStatData StatData { get; }
    public float BaseValue { get; set; }
    public float BaseBonus { get; set; }
    public float BasePercentBonus { get; set; }
    public float FlatBonus { get; set; }
    public float PercentBonus { get; set; }
    public float Total { get; }

    public float CalculateTotal();
}