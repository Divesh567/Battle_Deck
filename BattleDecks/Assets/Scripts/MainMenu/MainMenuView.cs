using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
/// <summary>
/// View Class of main menu contains all menu ui elements and UIInputs
/// </summary>
public class MainMenuView : MonoBehaviour
{
    //Events
    public class StartButtonPressedEvent : UnityEvent { }

    public StartButtonPressedEvent OnStartButtonPressed = new StartButtonPressedEvent();

    [SerializeField]
    private Button startButton;

    private void Start()
    {
        startButton.onClick.AddListener(() => StartButtonPressed());
    }

    private void StartButtonPressed()
    {
        startButton.gameObject.SetActive(false);
        OnStartButtonPressed.Invoke();
    }

    public void EnableDisableStartButton(bool isLimitReached)
    {
        startButton.interactable = isLimitReached;
    }
}
