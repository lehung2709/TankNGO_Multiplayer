using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingUI : MonoBehaviour
{
    public static LoadingUI Instance { get; private set; }

    [SerializeField] private GameObject loadingScreen;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    public void EnableLoadingUI()
    {
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);

        }
    }

    public void DisableLoadingUI()
    {
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false);
        }
    }

   
}
