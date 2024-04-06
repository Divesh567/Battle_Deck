using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This is the context of Main menu, is the highest level of class which contains all  mvc elements
/// </summary>
public class MainMenuContext : MonoBehaviour
{
    [SerializeField]
    private MainMenuView _view; //parent object

    private MainMenuController _controller;
    private MainMenuModel _model;

    private void OnEnable()
    {
        Initalize();
        EventManager.CardLimtReachedEvent += _controller.SwitchStartButton;
    }

    private void OnDisable()
    {
        EventManager.CardLimtReachedEvent -= _controller.SwitchStartButton;
    }


    private void Initalize()
    {
        _controller = new MainMenuController();
        _model = new MainMenuModel();

        _controller.InitalizeController(_view, _model);
    }

    

}
