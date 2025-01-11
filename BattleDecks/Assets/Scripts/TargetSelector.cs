using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TargetSelector : MonoBehaviour
{
    public List<ICardAffectable> cardAffectables = new List<ICardAffectable>();

    public static UnityAction<ICardAffectable> registerAction;
    public static UnityAction<ICardAffectable> unRegisterAction;
    public static UnityAction<CardModel> activateTargets;

    public static CardModel currentSelectedCard;

    private void OnEnable()
    {
        registerAction += RegisterTarget;
        unRegisterAction += UnRegister;
        activateTargets += ActivateTargetSelectors;
    }

    private void OnDisable()
    {
        registerAction -= RegisterTarget;
        unRegisterAction -= UnRegister;
        activateTargets -= ActivateTargetSelectors;
    }

    private void  RegisterTarget(ICardAffectable cardAffectable)
    {
        var targetContext = cardAffectable as BaseContext;

        TargetSelectorLogger.instance.Showlog("TARGET ADDED " + targetContext.gameObject.name);


        cardAffectables.Add(cardAffectable);
    }

    private void UnRegister(ICardAffectable cardAffectable)
    {
        var targetContext = cardAffectable as BaseContext;

        if (cardAffectables.Contains(cardAffectable))
        {
            cardAffectables.Remove(cardAffectable);

            TargetSelectorLogger.instance.Showlog("TARGET REMOVED " + targetContext.gameObject.name);
        }
    }

    private void ActivateTargetSelectors(CardModel selectedCard)
    {
        cardAffectables.ForEach(x => x.EnableSelector());
        currentSelectedCard = selectedCard;
    }
}
