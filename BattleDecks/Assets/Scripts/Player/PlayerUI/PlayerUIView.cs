using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using DG.Tweening;

public class PlayerUIView : MonoBehaviour
{
    [SerializeField]
    private Slider healthSlider;
    [SerializeField]
    private Slider staminaSlider;


    private PlayerUIModel uIModel;

    public void Init(PlayerUIModel model)
    {
        uIModel = model;

        uIModel.obvStamina.valueChanged.AddListener(OnStaminaReduced);
        uIModel.obvHealth.valueChanged.AddListener(OnHealthReduced);
    }

    private void OnHealthReduced(int amount)
    {
        healthSlider.value = amount;
    }

    private void Start()
    {
        healthSlider.value = 100;
        staminaSlider.value = 100;
    }

    void OnStaminaReduced(int amount)
    {
        staminaSlider.DOValue(amount, 0.5f);
    }
}