using Unity.Netcode;
using UnityEngine;

public class Stat : IStat
{
    public IStatData StatData { get; }

    public float BaseValue { get => baseValue; set { baseValue = value; isModified = true; } }
    public float BaseBonus { get => baseBonus; set { baseBonus = value; isModified = true; } }
    public float BasePercentBonus { get => basePercentBonus; set { basePercentBonus = value; isModified = true; } }
    public float FlatBonus { get => flatBonus; set { flatBonus = value; isModified = true; } }
    public float PercentBonus { get => percentBonus; set { percentBonus = value; isModified = true; } }
    public float Total
    {
        get
        {
            if (isModified)
            {
                total = CalculateTotal();
                isModified = false;
            }
            return total;
        }
    }


    private bool isModified;

    private float baseValue;
    private float baseBonus;
    private float basePercentBonus;
    private float flatBonus;
    private float percentBonus;
    private float total;


    public float CalculateTotal()
    {
        total = Mathf.Max(Mathf.Min(((BaseValue + BaseBonus) * (1 + BasePercentBonus) + FlatBonus) * (1 + PercentBonus), StatData.MaxValue), StatData.MinValue);
        total = StatData.UseRoundToInt ? Mathf.RoundToInt(total) : total;
        return StatData.Formatter.Format(total);
    }


    public Stat(float baseValue, IStatData statData)
    {
        BaseValue = baseValue;
        StatData = statData;
    }


}