using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class QuitGameHandler : NetworkBehaviour
{
    
   public void OnQuitGameClick()
   {

        AudioManager.Instance.PlayBtnSound();

        if (IsServer)
        {
            HostSingleton.Instance.GameManager.Shutdown();
        }
        else
        {
            ClientSingleton.Instance.GameManager.NetworkClient.Disconnect();
                
        }

    }        
}

