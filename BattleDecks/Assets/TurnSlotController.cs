using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Events;

public class TurnSlotController : MonoBehaviour
{
    [SerializeField]
    private List<Transform> slots;


    public class NextTurnAnimationComeplete : UnityEvent { }

    public NextTurnAnimationComeplete OnNextTurnAnimationComplete = new NextTurnAnimationComeplete();



    public Transform FindNextEmptySlot()
    {
        for(int i = 0; i < slots.Count; i++)
        {
            if(slots[i].childCount == 0)
            {
                return slots[i];
            }
        }

        return null;
    }




    public void AnimateNextTurn() //TO BE SOLVED
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if(i == 0)
            {
                slots[i].GetComponentInChildren<Image>().DOFade(0, 0.5f).OnComplete(() =>
                {
                    if (slots[0].childCount > 0)
                    {
                        Destroy(slots[0].GetChild(0).gameObject);
                    }
                });

                continue;
            }

            if (slots[i].childCount != 0)
            {
                Transform child = slots[i].GetChild(0);
                child.SetParent(slots[i - 1]);

                // Move the child to the new parent and animate its position
                RectTransform rectTransform = child.GetComponent<RectTransform>();
                rectTransform.DOAnchorPos(Vector3.zero, 0.5f);
            }
        }

        OnNextTurnAnimationComplete.Invoke();
    }
}
