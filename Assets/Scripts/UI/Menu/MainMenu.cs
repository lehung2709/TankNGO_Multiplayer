using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField joinCodeField;

    private void Start()
    {
        LoadingUI.Instance.DisableLoadingUI();
    }

    public async void StartHost()
    {
        LoadingUI.Instance.EnableLoadingUI();
        await HostSingleton.Instance.GameManager.StartHostAsync();

    }

    public async void StartClient()
    {
        LoadingUI.Instance.EnableLoadingUI();
        await ClientSingleton.Instance.GameManager.StartClientAsync(joinCodeField.text);

    }
}

