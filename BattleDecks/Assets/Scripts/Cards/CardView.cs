using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

//Card View contains the UI logic and component references of a card
public class CardView : BaseView
{
    public class CardSelected : UnityEvent {}

    public class CardUnSelected : UnityEvent {}

    public class CardUsed : UnityEvent {}

    //EVENTS
    public CardSelected CardSelectedEvent = new CardSelected();
    public CardUnSelected CardUnSelectedEvent = new CardUnSelected();
    public CardUsed CardUsedEvent = new CardUsed();

    [SerializeField]
    private TextMeshProUGUI cardNameTextMesh;

    [SerializeField]
    public Button selectButton;

    public Transform _startingParent;
    public Vector3 _startingPos;

    public CardModel cardModel;
    public CardModel CardModel { get { return cardModel; } set 
    {
        cardModel = value;
        SetCardDetails();
    }}
    
    private void Start()
    {
        _startingParent = transform.parent;
        _startingPos = transform.position;

        selectButton.onClick.AddListener(() => CardSelectedEvent.Invoke());

    }

    public void SetCardDetails()
    {
        cardNameTextMesh.text = CardModel.cardName;
    }


    public void UnSelectCard()
    {
        transform.SetParent(_startingParent);
        transform.position = _startingPos;
        selectButton.image.color = Color.white;

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => CardSelectedEvent.Invoke());

    }

    public void UpdateButtonOnCardSelected()
    {
        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => CardUnSelectedEvent.Invoke());
        selectButton.image.color = Color.cyan;
    }

    public void SetCardsOnGameStart()
    {
        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => Usecard());
    }


    private void Usecard()
    {
        CardUsedEvent.Invoke();

        Destroy(this.gameObject);
    }
    public void DisableCard()
    {
        gameObject.SetActive(false);
    }


}
