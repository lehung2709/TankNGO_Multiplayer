using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class HostSingleton : MonoBehaviour
{
    public static HostSingleton Instance { get; private set; }

    public HostGameManager GameManager { get; private set; }

   

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);

        }
        else
        {

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void CreateHost()
    {
        GameManager = new HostGameManager();
    }

    private void OnDestroy()
    {
        GameManager?.Dispose();
    }
}

