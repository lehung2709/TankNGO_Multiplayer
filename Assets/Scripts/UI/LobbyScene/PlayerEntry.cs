using System.Collections;
using TMPro;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEntry : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameTMP;
    [SerializeField] private TextMeshProUGUI playerReadyStatusTMP;
    [SerializeField] private Button kickBtn;

    private NetworkVariable<bool> isReady = new NetworkVariable<bool>(false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    public bool IsReady {  get { return isReady.Value; } }

    private NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>("",
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        if (IsServer && !IsOwner)
        {
            kickBtn.gameObject.SetActive(true);
            kickBtn.onClick.AddListener(Kick);
        }
        isReady.OnValueChanged += OnReadyStatusChanged;
        playerName.OnValueChanged += OnPlayerNameChange;
        playerReadyStatusTMP.text = isReady.Value ? "Ready" : "Not Ready";
        playerNameTMP.text = playerName.Value.ToString();


        StartCoroutine(WaitTillLobbyManagerReady());


    }

    private IEnumerator WaitTillLobbyManagerReady()
    {
        
        yield return new WaitUntil(() => LobbyReadyManager.Instance != null);

        if (!IsServer && IsOwner)
        {

            LobbyReadyManager.Instance.SetOwnerPlayerEntry(this);
        }
        if (IsClient)
        {
            LobbyReadyManager.Instance.SetParennt(transform);
        }
    }    


    public void SetPlayerName(string name)
    {
        playerName.Value = name;

    }
    private void OnPlayerNameChange(FixedString32Bytes oldValue,FixedString32Bytes newValue)
    {
        playerNameTMP.text = newValue.ToString();

    }

    [ServerRpc]
    public void ToggleReadyStatusServerRPC()
    {
        
         isReady.Value = !isReady.Value;
        LobbyReadyManager.Instance.ChangeReadyCount(isReady.Value);

    }
   


    private void OnReadyStatusChanged(bool oldValue, bool newValue)
    {
        playerReadyStatusTMP.text = newValue ? "Ready" : "Not Ready";
    }

    private void Kick()
    {
        LobbyReadyManager.Instance.ClientDisconectedServerRPC(IsReady);
        NetworkManager.Singleton.DisconnectClient(OwnerClientId);
    }

}
