using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected void Start()
    {
        PlayMusic("MainTheme");
    }

    public void PlayMusic(string target)
    {
        Sound s = Array.Find(musicSounds, sound => sound.name == target);

        Assert.IsNotNull(s, "Music " + target + " not found");

        if (s != null)
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }

    public void PlaySFX(string target)
    {
        Sound s = Array.Find(sfxSounds, sound => sound.name == target);

        Assert.IsNotNull(s, "Sound " + target + " not found");

        if (s != null)
        {
            sfxSource.clip = s.clip;
            sfxSource.PlayOneShot(sfxSource.clip);
        }
    }

    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;   
    }

    public void ToggleSFX()
    {
        sfxSource.mute = !sfxSource.mute;
    }

    public void MusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}
