using UnityEngine;
using UnityEngine.UI;

public class MasterVolumeAdjuster : MonoBehaviour
{
    private Slider slider;


    private void Awake()
    {
        slider = GetComponent<Slider>();

    }


    private void OnEnable()
    {
        slider.value = AudioManager.Instance.MasterVol;
    }


    public void SetMasterVolume()
    {
        AudioManager.Instance.SetMasterVolume(slider.value);
    }
}
