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

    private void Start()
    {
        healthSlider.value = 100;
        staminaSlider.value = 100;
    }

    public void Init(PlayerUIModel model)
    {
        uIModel = model;

        uIModel.obvStamina.valueChanged.AddListener(OnStaminaChanged);
        uIModel.obvHealth.valueChanged.AddListener(OnHealthChanged);
    }

    private void OnHealthChanged(int amount)
    {
        healthSlider.DOValue(amount, 0.5f);
    }

   

    void OnStaminaChanged(int amount)
    {
        staminaSlider.DOValue(amount, 0.5f);
    }
}