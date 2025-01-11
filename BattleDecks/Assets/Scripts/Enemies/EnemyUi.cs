using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUi : MonoBehaviour
{
    public Canvas canvas;
    public Slider healthSlider;


    public void  InitUi(EnemyModel enemyModel)
    {
        healthSlider.maxValue = enemyModel.enemyStats.MaxHealth;
        healthSlider.value = enemyModel.enemyStats.MaxHealth;
        canvas.worldCamera = UICameraControl.Instance.cam;
    }


    public void OnHealthReduced(int Amount)
    {
        healthSlider.DOValue(Amount, 0.5f);

    }
}