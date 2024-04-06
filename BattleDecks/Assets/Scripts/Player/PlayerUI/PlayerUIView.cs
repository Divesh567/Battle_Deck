using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerUIView : MonoBehaviour
{
    [SerializeField]
    private Slider healthSlider;
    [SerializeField]
    private Slider staminaSlider;


    private void Start()
    {
        healthSlider.value = 100;
        staminaSlider.value = 100;
    }
}