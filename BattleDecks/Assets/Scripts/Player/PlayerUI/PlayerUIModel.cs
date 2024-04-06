using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerUIModel", menuName = "Player/UI/PlayerUIModel", order = 2)]

public class PlayerUIModel : ScriptableObject
{
    public class StaminaReduced : UnityEvent<int> { }
    public StaminaReduced StaminaConsumedEvent = new StaminaReduced();

    public int stamina = 100;


    public void InitModel()
    {
        StaminaConsumedEvent.AddListener(OnStaminaReduced);
    }


    void OnStaminaReduced(int amount)
    {
        stamina -= amount;
        Debug.Log(stamina);
    }
}