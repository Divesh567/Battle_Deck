using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Contains the ui logic/Components of Deck Context
public class DeckView : BaseView
{
    private List<Transform> slots = new List<Transform>();
    public List<CardView> cards =  new List<CardView>();


    [SerializeField]
    private Transform slotParent;
    [SerializeField]
    private TextMeshProUGUI deckSizeTMP;

    private int deckSize;

    public void InitDeckView(DeckModel deckModel)
    {
        deckSize = deckModel.SizeLimit;

        for(int i = 0; i < deckSize; i++)
        {
            var newSlot = Instantiate(new GameObject("slot32132"), slotParent);
             newSlot.AddComponent<RectTransform>();
            slots.Add(slotParent.GetChild(i));
        }

        UpdateDeckSizeTMP();
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

        deckSizeTMP.text = $"{cards}/{deckSize}";
    }

}
