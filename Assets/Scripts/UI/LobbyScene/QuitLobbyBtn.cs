using UnityEngine;
using UnityEngine.UI;

public class QuitLobbyBtn : MonoBehaviour
{
    private Button btn;

    private void Awake()
    {
        btn = GetComponent<Button>();
    }
    private void Start()
    {
        btn.onClick.AddListener(OnQuitClick);
    }

    private void OnQuitClick()
    {
        AudioManager.Instance.PlayBtnSound();

        LobbyReadyManager.Instance.QuitLobby();
    }
}
