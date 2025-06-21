using System.Collections;
using TMPro;
using UnityEngine;

public class DeadUI : MonoBehaviour
{ 
    [SerializeField] private GameObject deadPanel;
    [SerializeField] private TextMeshProUGUI countDownTMP;
    public void StartCountDown(float duration)
    {
        deadPanel.SetActive(true);
        StartCoroutine(CountDownCoroutine(duration));
    }

    private IEnumerator CountDownCoroutine(float duration)
    {
        float count = duration;
        countDownTMP.text = count.ToString();
        while (count > 0)
        {
            yield return new WaitForSeconds(1f);
            count--;
            countDownTMP.text=count.ToString();
        }
        yield return new WaitForSeconds(0.2f);
        deadPanel.SetActive(false);
    }

}
