using UnityEngine.Events;

//Contains the data of deck on main menu
public class DeckModel 
{
    public class DeckLimitReached : UnityEvent { }
    public class DeckLimitNotReached : UnityEvent { }

    public DeckLimitReached DeckLimitReachedEvent = new DeckLimitReached();

    public DeckLimitNotReached DeckLimitNotRecheadEvent = new DeckLimitNotReached();


    private int sizeLimit = 7;
    private int currentSize;

    public int SizeLimit { get { return sizeLimit; } set { sizeLimit = value; } }
    public int CurrentSize { get { return currentSize; } set { currentSize = value; } }

    public void OnDeckSizeChanged(int value)
    {
        currentSize += value;
        if (CurrentSize == SizeLimit)
        {
            DeckLimitReachedEvent.Invoke();

        }
        else
        {
            DeckLimitNotRecheadEvent.Invoke();
        }
    }

}
