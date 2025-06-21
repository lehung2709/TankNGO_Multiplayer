using TMPro;
using UnityEngine;

public class EndGameUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI winerTMP;
    [SerializeField] private GameObject endGamePanel;
    public void EndGame(string winnerPlayer)
    {
        endGamePanel.gameObject.SetActive(true);
        winerTMP.text = winnerPlayer;
    }    
}
