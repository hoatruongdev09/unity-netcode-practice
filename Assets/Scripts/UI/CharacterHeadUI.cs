using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHeadUI : MonoBehaviour
{
    [SerializeField] private HeadHpBar hpBar;

    public void SetHeadHP(float? lastHp, float currentHp)
    {
        hpBar.SetPercent(lastHp, currentHp);
    }
}
