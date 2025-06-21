using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReadyBtn : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_TextMeshPro;
    
    private Image Image;

    private Button btn;

    private void Awake()
    {
        Image = GetComponent<Image>();
        btn = GetComponent<Button>();
    }
    private void Start()
    {
        btn.onClick.AddListener(OnReadyBtnClick);
    }

    private void OnReadyBtnClick()
    {
        if(LobbyReadyManager.Instance.ToggleReady())
        {
            Image.color = Color.red;
            m_TextMeshPro.text = "NoReady";
        }    
        else
        {
            Image.color= Color.green;
            m_TextMeshPro.text = "Ready";
        }    
    }    


}
