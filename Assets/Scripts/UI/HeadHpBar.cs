using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class HeadHpBar : MonoBehaviour
{

    [SerializeField] private Image currentHpSlider;
    [SerializeField] private Image lastHpSlider;

    [SerializeField] private float smoothSpeed = 2;
    private Tween hpTween;

    public void SetPercent(float? lastPercent, float newPercent)
    {
        if (lastPercent.HasValue)
        {
            lastHpSlider.fillAmount = lastPercent.Value;
        }
        currentHpSlider.fillAmount = newPercent;
    }

    private void Update()
    {
        lastHpSlider.fillAmount = Mathf.Lerp(lastHpSlider.fillAmount, currentHpSlider.fillAmount, smoothSpeed * Time.deltaTime);
    }
}