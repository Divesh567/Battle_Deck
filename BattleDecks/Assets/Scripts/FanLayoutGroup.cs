using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UnityEngine.Events;

[ExecuteInEditMode]
public class FanLayoutGroup : MonoBehaviour
{
    public static UnityEvent OnGroupChangedEvent = new UnityEvent();
    [SerializeField]
    private RectTransform parentPanel;


    private List<RectTransform> _childrenList;

    [Header("Layout Values")]
    [SerializeField]
    private float _layoutRotation;
    [SerializeField]
    private float _spacing;

    [Header("Animation Values")]
    [SerializeField]
    private float _animationSpeed;

    private float _cardStartPosition;
    private float _cardSpacing;
    private float positionOffsetX;
    private float _rotationOffset;
    private float CardHorizontalOffset;


    private bool isEven;

  
    private List<Vector2> newPosForCards = new List<Vector2>();
    private List<Quaternion> newRotForCards =  new List<Quaternion>();

    private void OnEnable()
    {
        OnGroupChangedEvent.AddListener(CheckCards);
    }

    private void OnDisable()
    {
        OnGroupChangedEvent.AddListener(CheckCards);
    }

    public void CheckCards()
    {
        Debug.Log("FANLAYOUT UPDATED");
        _childrenList = new List<RectTransform>();
        
        for(int i = 0; i < transform.childCount; i++)
        {
            _childrenList.Add(transform.GetChild(i).GetComponent<RectTransform>());
        }
       

        if (_childrenList.Count == 0)
        {
            return;
        }
        if (_childrenList.Count % 2 == 0)
        {
            isEven = true;
        }
        else
        {
            isEven = false;
        }

        positionOffsetX = parentPanel.sizeDelta.x / _childrenList.Count / positionOffsetX;
        AnchorCards();
        _cardSpacing = _spacing / _childrenList.Count;
    }

    public void AnchorCards()
    {
        foreach (RectTransform card in _childrenList)
        {
            card.anchorMin = new Vector2(0, 0.5f);
            card.anchorMax = new Vector2(0, 0.5f);
        }

        SetupCards();
    }

    public void SetupCards()
    {
        newPosForCards.Clear();

        _cardStartPosition = (parentPanel.sizeDelta.x / _childrenList.Count - _cardSpacing);

        var lastCardPos = (parentPanel.sizeDelta.x - _cardStartPosition);

        var lengthofhand = (lastCardPos - _cardStartPosition);

        CardHorizontalOffset = lengthofhand / (/*transform.childCount*/_childrenList.Count - 1);

        newPosForCards.Add(new Vector2(_cardStartPosition, 0f));

        var pos = newPosForCards[0];

        if(_childrenList.Count == 0)
        {
            return;
        }
        if (_childrenList.Count == 1)
        {
            SetOneCard();
            return;
        }
        if (_childrenList.Count == 2)
        {
            FanOutTwoCards();
        }
        else
        {
            for (int i = 1; i <= _childrenList.Count - 1; i++)
            {
                newPosForCards.Add(new Vector2(pos.x + CardHorizontalOffset, 0));
                pos = newPosForCards[i];
            }
            FanOutCards();
        }
    }

    private void SetOneCard()
    {
        var cardPosX = parentPanel.sizeDelta.x / 2;
        _childrenList[0].DOAnchorPos(new Vector2(cardPosX, 0f), 1f);
        _childrenList[0].DOLocalRotate(Vector3.zero, 1f);
    }

    private void FanOutTwoCards()
    {
        _cardStartPosition = parentPanel.sizeDelta.x / _childrenList.Count;

        _childrenList[0].DOAnchorPos(new Vector2(_cardStartPosition - _cardSpacing * 4, 0f), 0.5f);
        _childrenList[1].DOAnchorPos(new Vector2(_cardStartPosition + _cardSpacing * 4, 0f), 0.5f);
        var firtRot = Quaternion.Euler(0, 0, _layoutRotation / 2).eulerAngles;
        _childrenList[0].DOLocalRotate(firtRot, 0.5f);
        var secondRot = Quaternion.Euler(0, 0, -_layoutRotation / 2).eulerAngles;
        _childrenList[1].DOLocalRotate(secondRot, 0.5f);

    }
    public void FanOutCards()
    {
       

        newRotForCards.Clear();
        for (int i = 0; i <= _childrenList.Count - 1; i++)
        {
            var emptyRot = Quaternion.Euler(Vector3.zero);
            newRotForCards.Add(emptyRot);
        }
        var firstCard = _childrenList[0];

        _rotationOffset = _layoutRotation / (_childrenList.Count / 2);
        var firstRot = Quaternion.Euler(0, 0, _layoutRotation);
        newRotForCards[0] = firstRot;

        var previousCard = firstCard;
        var previousRot = newRotForCards[0];

        for (int i = 1; i <= _childrenList.Count - 1; i++)
        {
            if (i <= _childrenList.Count / 2)
            {
                var nextRotValue = Quaternion.Euler(0, 0, previousRot.eulerAngles.z - _rotationOffset);
                newRotForCards[i] = nextRotValue;
                // _childrenList[i].rotation = nextRotValue;
                previousCard = _childrenList[i];
                previousRot = newRotForCards[i];
            }
            else
            {
              
                var relativeCard = newRotForCards[_childrenList.Count - i];
                newRotForCards[i - 1] = (Quaternion.Euler(0, 0, -relativeCard.eulerAngles.z));
            }
        }

        var lastCard = _childrenList.FindLast(x => x);
        var lastRot = Quaternion.Euler(0, 0, -_layoutRotation);

        newRotForCards[_childrenList.Count - 1] = lastRot;

        for (int i = 0; i <= _childrenList.Count - 1; i++)
        {
            _childrenList[i].DOLocalRotate(newRotForCards[i].eulerAngles, _animationSpeed);

            if (i == _childrenList.Count / 2 && !isEven)
            {
                _childrenList[i].DOLocalRotate(Quaternion.Euler(Vector2.zero).eulerAngles, _animationSpeed);
            }
        }

        SetCardVeritcalOffset();

    }

    public void SetCardVeritcalOffset()
    {
      
        newPosForCards[0] = new Vector2(newPosForCards[0].x, 0f);
        
      
        var previousRot = newRotForCards[0];
        var previousPos = newPosForCards[0];


        for (int i = 1; i <= _childrenList.Count - 1; i++)
        {
            if (i <= _childrenList.Count / 2)
            {

                var angle = previousRot.eulerAngles.z;
                var degToRad = angle * Mathf.Deg2Rad;
                var cardYPos = Mathf.Abs(CardHorizontalOffset * Mathf.Tan(degToRad));
                newPosForCards[i] = new Vector2(newPosForCards[i].x, previousPos.y + cardYPos);
                previousRot = newRotForCards[i];
                previousPos = newPosForCards[i];
            }
            else
            {
             
                var relativeCard = newPosForCards[newPosForCards.Count - i];
                newPosForCards[i - 1] = new Vector2(newPosForCards[i - 1].x, relativeCard.y);
            }
        }

        newPosForCards[newPosForCards.Count - 1] = new Vector2(newPosForCards[newPosForCards.Count - 1].x, 0f);

        if (!isEven)
        {
            var middleCard = newPosForCards[newPosForCards.Count / 2];
            previousPos = newPosForCards[newPosForCards.Count / 2 - 1];
            previousRot = newRotForCards[newPosForCards.Count / 2 - 1];
            var angle = previousRot.eulerAngles.z;
            var degToRad = angle * Mathf.Deg2Rad;
            var cardYPos = Mathf.Abs(CardHorizontalOffset * Mathf.Tan(degToRad)) / 2;
            middleCard = new Vector2(middleCard.x, previousPos.y + cardYPos);
            newPosForCards[newPosForCards.Count / 2] = middleCard;

        }
        for (int i = 0; i <= _childrenList.Count - 1; i++)
        {
            _childrenList[i].DOAnchorPos(newPosForCards[i], _animationSpeed);
        }


    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckCards();
        }
    }

    public void RemoveCard(int ChildIndex)
    {
        _childrenList.RemoveAt(ChildIndex);
        CheckCards();
    }

}