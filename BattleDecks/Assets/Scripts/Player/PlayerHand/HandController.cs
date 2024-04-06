using UnityEngine.Events;
/// <summary>
/// Controller class for hand context controls flow of hand in game
/// </summary>
public class HandController
{
    private HandView _view;

    public class CardPlayed : UnityEvent<CardModel>{}
    public CardPlayed CardPlayedEvent = new CardPlayed();

    public void Initialize(HandView view)
    {
        _view = view;

        _view.CardPlayedEvent.AddListener(PlayCard);

        SpawnCards();
    }
    public void SpawnCards()
    {
        var cardList = PersistentData.selectedCards;
        _view.SpawnCards(cardList);
    }

    private void PlayCard(CardModel model)
    {
        CardPlayedEvent.Invoke(model);
    }
}
