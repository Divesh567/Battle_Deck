using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIContext : MonoBehaviour
{
    [SerializeField]
    private PlayerUIView _view;
    private PlayerUIController controller;
    [SerializeField]
    private PlayerUIModel _model;

    private void Awake()
    {
        Initalizie();
    }

    private void OnEnable()
    {
        PlayerEventSystem.OnStaminaReducedEvent += controller.OnStaminaConsumedEvent;
    }
    private void Initalizie()
    {
        controller = new PlayerUIController();
        controller.Initalize(_model, _view);
    }
}
