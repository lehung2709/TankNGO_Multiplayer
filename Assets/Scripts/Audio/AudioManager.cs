using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private SoundLibSO soundLibSO;
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private string musicVolume = "Music";
    [SerializeField] private string SFXVolume = "SFX";
    [SerializeField] private string masterVolume = "Master";
    public float MusicVol { get; private set; } = 1f;
    public float SFXVol { get; private set; } = 1f;
    public float MasterVol { get; private set; } = 1f;

    [SerializeField] private Queue<AudioEmitter> audioEmitterPool;
    [SerializeField] private int poolSize = 10;
    [SerializeField] private int maxEmitters = 20;
    [SerializeField] private string musicName;





    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            InitializePool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadVolume();
        PlayMusic(musicName);

    }

    private void LoadVolume()
    {
        MusicVol = PlayerPrefs.GetFloat(musicVolume, 1f);
        SFXVol = PlayerPrefs.GetFloat(SFXVolume, 1f);
        MasterVol = PlayerPrefs.GetFloat(masterVolume, 1f);

        myMixer.SetFloat(musicVolume, Mathf.Log10(Mathf.Clamp(MusicVol, 0.0001f, 1f)) * 20f);
        myMixer.SetFloat(SFXVolume, Mathf.Log10(Mathf.Clamp(SFXVol, 0.0001f, 1f)) * 20f + 20f);
        myMixer.SetFloat(masterVolume, Mathf.Log10(Mathf.Clamp(MasterVol, 0.0001f, 1f)) * 20f);

    }

    public void SetMasterVolume(float volume)
    {
        MasterVol = volume;
        PlayerPrefs.SetFloat(masterVolume, volume);
        myMixer.SetFloat(masterVolume, Mathf.Log10(Mathf.Clamp(MasterVol, 0.0001f, 1f)) * 20f);

    }
    public void SetMusicVolume(float volume)
    {
        MusicVol = volume;
        PlayerPrefs.SetFloat(musicVolume, volume);
        myMixer.SetFloat(musicVolume, Mathf.Log10(Mathf.Clamp(MusicVol, 0.0001f, 1f)) * 20f);

    }
    public void SetSFXVolume(float volume)
    {
        SFXVol = volume;
        PlayerPrefs.SetFloat(SFXVolume, volume);
        myMixer.SetFloat(SFXVolume, Mathf.Log10(Mathf.Clamp(SFXVol, 0.0001f, 1f)) * 20f + 20f);

    }


    private void InitializePool()
    {
        audioEmitterPool = new Queue<AudioEmitter>();

        for (int i = 0; i < poolSize; i++)
        {
            CreateSoundEmitter();
        }
    }

    private AudioEmitter CreateSoundEmitter()
    {
        GameObject soundEmitterObject = new GameObject("SoundEmitter");
        AudioEmitter emitter = soundEmitterObject.AddComponent<AudioEmitter>();
        soundEmitterObject.SetActive(false);
        audioEmitterPool.Enqueue(emitter);
        return emitter;
    }

    public AudioEmitter SpawnSoundEmitter(Transform parent, string soundName, Vector3 pos)
    {
        AudioEmitter audioEmitter;

        if (audioEmitterPool.Count > 0)
        {
            audioEmitter = audioEmitterPool.Dequeue();
        }
        else if (audioEmitterPool.Count < maxEmitters)
        {
            audioEmitter = CreateSoundEmitter();
        }
        else
        {
            audioEmitter = audioEmitterPool.Dequeue();
        }

        audioEmitter.transform.SetParent(parent);
        audioEmitter.transform.position = pos;
        audioEmitter.gameObject.SetActive(true);

        audioEmitter.PlaySound(soundLibSO.GetSound(soundName));
        return audioEmitter;
    }

    public void ReturnToPool(AudioEmitter emitter)
    {
        audioEmitterPool.Enqueue(emitter);
    }



    public void PlayBtnSound()
    {
        SpawnSoundEmitter(null, "Btn", Vector3.zero);
    }

    public void PlayMusic(string musicName)
    {
        this.musicName = musicName;
        SpawnSoundEmitter(null, musicName, Vector3.zero);

    }
}
