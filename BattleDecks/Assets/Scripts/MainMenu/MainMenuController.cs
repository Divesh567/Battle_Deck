using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// The controller class for main menu, controls the ui flow of main menu
/// </summary>
public class MainMenuController 
{
    private MainMenuView _view;
    private MainMenuModel _model;


    public void InitalizeController(MainMenuView view, MainMenuModel model)
    {
        _view = view;
        _model = model;

        _view.OnStartButtonPressed.AddListener(() => OnStartButtonPressed());
    }

    private void OnStartButtonPressed()
    {
        EventManager.OnGameStarButtontEventCaller();
    }

    public void SwitchStartButton(bool isLimitReached)
    {
        _view.EnableDisableStartButton(isLimitReached);
    }
}
