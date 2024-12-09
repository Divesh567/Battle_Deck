using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

public class EnemyView : MonoBehaviour
{
    public class TargetSelected : UnityEvent { }
    public TargetSelected TargetSelectedEvent = new TargetSelected();

    public Collider Collider;

    public Slider healthSlider; // Add in new class


    private EnemyModel uIModel;

    public void Init(EnemyModel enemyModel)
    {
        uIModel = enemyModel;

     
        uIModel.obvHealth.valueChanged.AddListener(OnHealthReduced);
    }


    public void KillEnemey()
    {
        Destroy(this.gameObject);
    }

    private void OnMouseDown()
    {
        Collider.enabled = false;
        TargetSelectedEvent.Invoke();
    }

    private void OnHealthReduced(int Amount)
    {
        healthSlider.DOValue(Amount, 0.5f);

    }

    private void OnStaminaReduced()
    {


    }
}
