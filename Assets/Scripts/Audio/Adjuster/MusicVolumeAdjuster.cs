using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MusicVolumeAdjuster : MonoBehaviour
{

    private Slider slider;


    private void Awake()
    {
        slider = GetComponent<Slider>();

    }


    private void OnEnable()
    {
        slider.value = AudioManager.Instance.MusicVol;
    }


    public void SetMusicVolume()
    {
        AudioManager.Instance.SetMusicVolume(slider.value);
    }



}
