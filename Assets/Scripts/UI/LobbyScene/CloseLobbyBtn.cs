using UnityEngine;
using UnityEngine.UI;

public class CloseLobbyBtn : MonoBehaviour
{
    private Button btn;

    private void Awake()
    {
        btn = GetComponent<Button>();
    }
    private void Start()
    {
        btn.onClick.AddListener(OnCloseBtnClick);
    }

    private void OnCloseBtnClick()
    {
        AudioManager.Instance.PlayBtnSound();

        LoadingUI.Instance.EnableLoadingUI();
        LobbyReadyManager.Instance.CloseLobby();
    }
}
