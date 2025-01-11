using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

[Serializable]
public class CardUIElements : MonoBehaviour
{
    public TextMeshProUGUI TMP_cardName;
    public Image baseImage;
    public Image mainImage;
    public TextMeshProUGUI TMP_cardInfo;

    public void SetupElements(CardUIDetails cardUIDetails)
    {
        TMP_cardName.text = cardUIDetails.CardName;
        TMP_cardInfo.text = cardUIDetails.cardInfo;

        baseImage.sprite = cardUIDetails.baseArt;
        mainImage.sprite = cardUIDetails.mainArt;
    }
}