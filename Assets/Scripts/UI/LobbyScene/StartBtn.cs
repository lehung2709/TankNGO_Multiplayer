using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartBtn : MonoBehaviour
{


    private Button btn;

    private void Awake()
    {
        btn = GetComponent<Button>();
    }
    private void Start()
    {
        btn.onClick.AddListener(OnStartBtnClick);
    }

    private void OnStartBtnClick()
    {
        AudioManager.Instance.PlayBtnSound();
        LobbyReadyManager.Instance.StartGame();
    }
}
