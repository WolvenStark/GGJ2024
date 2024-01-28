using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Sound[] musicIntroSounds, sfxSounds, musicSounds;
    public static AudioSource musicIntroSource = null;
    public static AudioSource sfxSource = null;
    public static AudioSource musicSource = null;

    public string currentTheme = null;

    public static float musicClipLength = 0.0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            GameObject newObj = null;

            if (musicIntroSource == null)
            {
                newObj = new GameObject("musicIntroSource");
                newObj.transform.SetParent(gameObject.transform);
                musicIntroSource = newObj.AddComponent<AudioSource>();
            }
            if (sfxSource == null)
            {
                newObj = new GameObject("sfxSource");
                newObj.transform.SetParent(gameObject.transform);
                sfxSource = gameObject.AddComponent<AudioSource>();
            }
            if (musicSource == null)
            {
                newObj = new GameObject("musicSource");
                newObj.transform.SetParent(gameObject.transform);
                musicSource = gameObject.AddComponent<AudioSource>();
            }

            ResetSelf();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected void Reset()
    {
        ResetSelf();
    }

    protected void ResetSelf()
    {
        musicIntroSource.loop = false;
        sfxSource.loop = false;
        musicSource.loop = true;
    }

    protected void Start()
    {
        //PlayMusicIntro("MainTheme");
        PlayMusic("MainTheme");
    }

    public void PlayMusicIntro(string trackName)
    {
        Sound s = Array.Find(Instance.musicIntroSounds, sound => sound.name == trackName);

        Assert.IsNotNull(s, "Music Intro" + trackName + " not found");

        if (s != null)
        {
            musicIntroSource.clip = s.clip;
            musicIntroSource.Play();

            currentTheme = trackName;
        }
    }

    public static void PlayMusic(string trackName)
    {
        Debug.LogWarning("PlayMusic " + trackName);

        Sound s = Array.Find(Instance.musicSounds, sound => sound.name == trackName);

        Assert.IsNotNull(s, "Music " + trackName + " not found");

        if (s != null)
        {
            // Assign new music clip
            musicSource.clip = s.clip;

            musicSource.PlayOneShot(musicSource.clip, musicSource.volume);
            musicClipLength = musicSource.clip.length;

            if (musicSource.loop)
            {
                LoopCaller(trackName, musicClipLength);
            }
        }
    }

    public void PlaySFX(string trackName)
    {
        Sound s = Array.Find(Instance.sfxSounds, sound => sound.name == trackName);

        Assert.IsNotNull(s, "Sound " + trackName + " not found");

        if (s != null)
        {
            sfxSource.clip = s.clip;
            sfxSource.PlayOneShot(sfxSource.clip);
        }
    }

    public void ToggleMusic()
    {
        musicIntroSource.mute = !musicIntroSource.mute;
        musicSource.mute = !musicSource.mute;   
    }

    public void ToggleSFX()
    {
        sfxSource.mute = !sfxSource.mute;
    }

    public void MusicVolume(float volume)
    {
        musicIntroSource.volume = volume;
    }

    public void SFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }

    //public void Update()
    //{
    //    if (!musicSource.isPlaying)
    //    {
    //        time = time + loopPointSeconds;

    //        musicIntroSource.PlayScheduled(time);

    //        nextSource = 1 - nextSource; //Switch to other AudioSource
    //    }
    //}

    public static IEnumerator LoopMusic(string trackName, float clipLength)
    {
        Debug.LogWarning("Loop Music - Begin Waiting " + trackName + " for " + clipLength + " seconds");
        yield return new WaitForSeconds(clipLength);
        Debug.LogWarning("Loop Music - Finished Waiting " + trackName);

        PlayMusic(trackName);
    }

    public static void LoopCaller(string trackName, float clipLength)
    {
        Instance.StartCoroutine(LoopMusic(trackName, clipLength));
    }
}
