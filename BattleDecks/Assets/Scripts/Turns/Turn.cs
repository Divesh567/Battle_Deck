using Unity;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Turn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ITurnObject turnObject;

    public Image playerImage;
    [SerializeField] public TextMeshProUGUI playerName;

    private void Awake()
    {
        if (playerName != null)
        {
            playerName.gameObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowPlayerName();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HidePlayerName();
    }

    private void ShowPlayerName()
    {
        if (playerName != null && !playerName.gameObject.activeSelf)
        {
            playerName.gameObject.SetActive(true);
        }
    }

    private void HidePlayerName()
    {
        if (playerName != null && playerName.gameObject.activeSelf)
        {
            playerName.gameObject.SetActive(false);
        }
    }

    
}