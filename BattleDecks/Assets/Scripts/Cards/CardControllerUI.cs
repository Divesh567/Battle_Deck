using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//Controller of cards, contains logic of selection and controls the from model to view
public class CardControllerUI
{
    public List<CardView> _view;
    private List<CardModel> _cardModel;

    
    public void InitalizeController(List<CardView> view, List<CardModel> model)
    {
        _view = view;
        _cardModel = model;

        InitalizeView();
    }

    private void InitalizeView()
    {
        for(int i = 0; i < _cardModel.Count; i++)
        {
            _view[i].CardModel = _cardModel[i];
        }
        _view.ForEach(x => x.CardSelectedEvent.AddListener(() => CardSelected(x)));
    }

    private void CardSelected(CardView card)
    {
        EventManager.CardSelectedEventCaller(card);
    }

    public void CardUnSelected(CardView card)
    {
        card.UnSelectCard();
    }

}
