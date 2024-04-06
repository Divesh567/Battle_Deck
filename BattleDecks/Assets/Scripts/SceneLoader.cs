using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    int MainMenuIndex = 0;
    int GameIndex = 1;

    private void OnEnable()
    {
        DontDestroyOnLoad(this);
        EventManager.OnGameStartButtonClickEvent += LoadGameScene;
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene(GameIndex);
    }

}
