using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum SoundEnum
{
    EFFECT,
    BGM,
    END
}

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance = null;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AudioManager>();
            }
            return _instance;
        }
    }

    [SerializeField]
    private AudioMixer _masterMixer;

    [SerializeField]
    private AudioMixerGroup _bgmGroup;

    [SerializeField]
    private AudioMixerGroup _sfxGroup;

    [SerializeField] private AudioClip _lobbyBgmClip;
    [SerializeField] private AudioClip _gameBgmClip;

    public AudioClip uiClickSoundClip;

    public float soundFadeOnTime;

    private AudioSource[] _audioSources = new AudioSource[(int)SoundEnum.END];

    private void Awake() // Awake
    {
        string[] soundNames = System.Enum.GetNames(typeof(SoundEnum));
        for (int i = 0; i < soundNames.Length - 1; i++)
        {
            GameObject go = new GameObject { name = soundNames[i] };
            _audioSources[i] = go.AddComponent<AudioSource>();
            _audioSources[i].playOnAwake = false;
            _audioSources[i].outputAudioMixerGroup = (soundNames[i] == "BGM" ? _bgmGroup : _sfxGroup);
            go.transform.SetParent(transform);
        }

        _audioSources[(int)SoundEnum.BGM].loop = true;

        SceneManagement.Instance.OnGameSceneLoaded += PlayGameBGM;

    }

    private void PlayGameBGM()
    {
        Play(_gameBgmClip, SoundEnum.BGM);
    }

    public void Play(AudioClip audioClips, SoundEnum type = SoundEnum.EFFECT)
    {
        if (audioClips == null)
        {
            Debug.LogError("cannot find audioclips");
            return;
        }

        if (type == SoundEnum.BGM)
        {
            StopAllCoroutines();
            AudioSource audioSource = _audioSources[(int)SoundEnum.BGM];

            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.volume = 0;
            audioSource.clip = audioClips;
            audioSource.Play();

            StartCoroutine(SoundFade(true, _audioSources[(int)SoundEnum.BGM], soundFadeOnTime, 1, SoundEnum.BGM));
            StartCoroutine(SoundFade(false, _audioSources[(int)SoundEnum.BGM], soundFadeOnTime, 0, SoundEnum.BGM));
        }
        else
        {
            AudioSource audioSource = _audioSources[(int)SoundEnum.EFFECT];
            audioSource.PlayOneShot(audioClips);
        }
    }

    public void Stop()
    {
        foreach (var audioSource in _audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
    }

    public void Mute(SoundEnum type, bool mute)
    {
        _masterMixer.SetFloat(type.ToString().ToLower(), mute ? -80 : 0);
    }

    public void ClickSound()
    {
        Play(uiClickSoundClip, SoundEnum.EFFECT);
    }

    public void PlayLobbyBGM()
    {
        Play(_lobbyBgmClip, SoundEnum.BGM);
    }

    IEnumerator SoundFade(bool fadeIn, AudioSource source, float duration, float endVolume, SoundEnum type)
    {
        if (!fadeIn)
        {
            yield return new WaitForSeconds((float)(source.clip.length - duration));
        }

        float time = 0f;
        float startVolume = source.volume;

        while (time < duration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, endVolume, time / duration);
            yield return null;
        }

        if (!fadeIn)
            Play(source.clip, type);
    }
}