using System.Collections.Generic;
using UnityEngine;

public class Test_AddCards : MonoBehaviour
{
    public List<CardModel> cardModels;

    private void Awake()
    {
        PersistentData.selectedCards = cardModels;
    }
}
