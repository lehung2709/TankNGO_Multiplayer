using TMPro;
using Unity.Cinemachine;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class TankPlayer : NetworkBehaviour
{
    private NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>("",
       NetworkVariableReadPermission.Everyone,
       NetworkVariableWritePermission.Server);

    [SerializeField] private TextMeshProUGUI playerNameTMP;
    [SerializeField] private CinemachineCamera virtualCam;
    [SerializeField] private SpriteRenderer minimapIconRenderer;
    [SerializeField] private int ownerPriority = 15;
    [SerializeField] private Color ownerColor= Color.blue;

    

    public override void OnNetworkSpawn()
    {
        if(IsOwner)
        {
            virtualCam.Priority = ownerPriority;
            minimapIconRenderer.color = ownerColor;
        }
       playerName.OnValueChanged += OnPlayerNameChanged;

       OnPlayerNameChanged("", playerName.Value.ToString());
    }

    private void OnPlayerNameChanged(FixedString32Bytes oldName, FixedString32Bytes newName)
    {
        playerNameTMP.text = newName.ToString();
    }

    
    public void SetPlayerName(string playerName)
    {
       this.playerName.Value = playerName;
    }
}
