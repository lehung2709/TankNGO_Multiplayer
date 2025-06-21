using UnityEngine;
using UnityEngine.UI;


public class QuitGameBtn : MonoBehaviour
{
    [SerializeField] private Button quitButton;

    void Start()
    {
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
        else
        {
            Debug.LogWarning("Quit Button is not assigned!");
        }
    }

    void QuitGame()
    {
        Debug.Log("Quitting the game...");
        Application.Quit();
    }
}
