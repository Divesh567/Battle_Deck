using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "Card Selection Animation", menuName = "Animations/Cards/Card Selection Animation", order = 0)]
public class CardSelectionAnimation : AnimationClass
{
    CardView _cardView;
    public string animIDPrefix;


    public CardSelectionAnimation(CardView cardView)
    {

        Debug.Log("CARD INITIALZTION ANMI");

        _cardView = cardView;
        animID = (animIDPrefix + _cardView.gameObject.GetInstanceID().ToString());

        OnPlayAnimationEvent.AddListener(CardSelectedAnimation);
        OnStopAnimationEvent.AddListener(CardUnselectedAnimation);
    }

    public void CardSelectedAnimation()
    {
        Debug.Log("CARD SELECTED ANMI");

        DOTween.Kill(animID);

        _cardView.image.color = Color.cyan;
        _cardView.rectTransform.DOAnchorPosY(30, 0.5f).SetRelative().SetId(animID);
    }

    public void CardUnselectedAnimation()
    {
        Debug.Log("This card Animation Stopeed");


        DOTween.Kill(animID);

        _cardView.image.color = Color.white;
        _cardView.rectTransform.DOAnchorPosY(-30, 0.5f).SetRelative().SetId(animID);
    }




}
