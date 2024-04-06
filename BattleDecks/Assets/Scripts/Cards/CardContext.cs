using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//The highest level in cards system 
public class CardContext : MonoBehaviour
{
    [SerializeField]
    private List<CardView> _view;

    private CardControllerUI controller;

    [SerializeField]
    private List<CardModel> _cardModel;

    private void OnEnable()
    {
        Initalize();
        EventManager.CardUnSelectedEvent += controller.CardUnSelected;
    }

    private void OnDisable()
    {
        EventManager.CardUnSelectedEvent -= controller.CardUnSelected;
    }

    private void Initalize()
    {
        controller = new CardControllerUI();
        controller.InitalizeController(_view, _cardModel);
    }


}
