using UnityEngine;
using UnityEngine.UI;

public class SFXVolumeAdjuster : MonoBehaviour
{
    private Slider slider;


    private void Awake()
    {
        slider = GetComponent<Slider>();

    }


    private void OnEnable()
    {
        slider.value = AudioManager.Instance.SFXVol;
    }


    public void SetSFXVolume()
    {
        AudioManager.Instance.SetSFXVolume(slider.value);
    }
}
