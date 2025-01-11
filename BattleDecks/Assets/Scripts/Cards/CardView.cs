using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

//Card View contains the UI logic and component references of a card
public class CardView : BaseView
{
    #region EVENTS
    public class CardSelected : UnityEvent {}
    public class CardUnSelected : UnityEvent {}
    public class CardUsed : UnityEvent {} 


    public CardSelected CardSelectedEvent = new CardSelected();
    public CardUnSelected CardUnSelectedEvent = new CardUnSelected();
    public CardUsed CardUsedEvent = new CardUsed();

    #endregion


    #region AnimationCache

    public RectTransform rectTransform;
    public Image image;
    public CanvasGroup canvasGroup;

    public CardSelectionAnimation selectionAnimation ;

    #endregion


    [SerializeField]
    private TextMeshProUGUI cardNameTextMesh;

    [SerializeField]
    public Button selectButton;

    public Transform _startingParent;
    public Vector3 _startingPos;

    public CardModel cardModel;

    public CardUIElements cardUI;


    public CardModel CardModel { get { return cardModel; } set 
    {
        cardModel = value;
        SetCardDetails();
    }}
    
    private void Start()
    {
        selectionAnimation = new CardSelectionAnimation(this);

        _startingParent = transform.parent;
        _startingPos = transform.position;

        selectButton.onClick.AddListener(() => CardSelectedEvent.Invoke());

        CardUsedEvent.AddListener(Usecard);
    }

    public void SetCardDetails()
    {
        cardUI.SetupElements(cardModel.cardUIDetails);
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
    }

    public void SetCardsOnGameStart()
    {
        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => OnCardSelected());
    }

    public void OnCardSelected()
    {
        CardSelectedEvent.Invoke();
    }

    private void Usecard()
    {
       
    }


    public void DisableCard()
    {
        selectButton.interactable = false;
    }

    public void EnableCard()
    {
        selectButton.interactable = true;
    }
}
