using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    public List<CardView> allCards = new List<CardView>();
    public Button button;
    public bool isUIActive;
    public GameObject inventoryView;
    public void AddCardToInventory(CardView cardView)
    {
        cardView.DisableCard();

        allCards.Add(cardView);
    }


    public void RemoveCardFromInventory(int index)
    {
        allCards[index].EnableCard();

        allCards.RemoveAt(index);
    }

    public void ToggleUiInvetory()
    {
        if (isUIActive)
        {
            inventoryView.SetActive(false);
            isUIActive = false;
        }
        else
        {
            isUIActive = true;
            inventoryView.SetActive(true);
        }
    }
}
