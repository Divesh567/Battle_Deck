using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Contains the ui logic/Components of Deck Context
public class DeckView : BaseView
{
    private List<Transform> slots;
    public List<CardView> cards =  new List<CardView>();


    [SerializeField]
    private Transform slotParent;
    [SerializeField]
    private TextMeshProUGUI deckSizeTMP;

    

    private void Start()
    {
        slots = new List<Transform>();

        for(int i = 0; i < slotParent.childCount; i++) //TODO should run according to size limit of deck model
        {
            slots.Add(slotParent.transform.GetChild(i));
        }
    }


    public void AddCardToDeck(CardView cardView)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if(slots[i].childCount == 0)
            {
                cardView.transform.SetParent(slots[i].transform);
                cardView.transform.localPosition = Vector3.zero;
                cards.Add(cardView);

                UpdateDeckSizeTMP();

                return;

            }   
        }
       
    }

    public void RemoveCard(CardView cardView)
    {
        var card = cards.FindIndex(x => x.gameObject.GetInstanceID() == cardView.gameObject.GetInstanceID());
        cards.RemoveAt(card);

        UpdateDeckSizeTMP();
    }


    public void UpdateDeckSizeTMP()
    {
        int cards = 0;

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].childCount > 0)
            {
                cards++;
            }
        }

        deckSizeTMP.text = cards + "/3";
    }

}
