using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnContext : MonoBehaviour
{

    private TurnModel turnModel = new TurnModel();
    private TurnController turnController;
    [SerializeField]
    private TurnView turnView;


    private void OnEnable()
    {
        TurnEventSystem.AddObjectsEvent += AddTurnObjects;
    }

    private void OnDisable()
    {
        TurnEventSystem.AddObjectsEvent -= AddTurnObjects;
    }
    private void Awake()
    {
        turnController = new TurnController();
        turnController.Initialzie(turnModel, turnView);
    }

    private void AddTurnObjects(TurnClass turnObject)
    {
        turnController.AddTurnObject(turnObject);

    }
}
