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

    public static string currentTheme = null;

    public static float musicIntroClipLength = 0.0f;
    public static float musicClipLength = 0.0f;
    public static float musicMinVolume = 0.0f;
    public static float musicMaxVolume = 1.0f;
    public static float musicClipFadeInLength = 0.05f;

    private static bool keepFadingIn = false;
    private static bool keepFadingOut = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            GameObject newObj = null;

            if (musicIntroSource == null)
            {
                newObj = null;
                newObj = new GameObject("musicIntroSource");
                newObj.transform.SetParent(gameObject.transform);
                musicIntroSource = newObj.AddComponent<AudioSource>();
            }
            if (sfxSource == null)
            {
                newObj = null;
                newObj = new GameObject("sfxSource");
                newObj.transform.SetParent(gameObject.transform);
                sfxSource = newObj.AddComponent<AudioSource>();
            }
            if (musicSource == null)
            {
                newObj = null;
                newObj = new GameObject("musicSource");
                newObj.transform.SetParent(gameObject.transform);
                musicSource = newObj.AddComponent<AudioSource>();
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
        musicIntroSource.playOnAwake = false;
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.playOnAwake = false;
    }

    protected void Start()
    {
        PlaySFX("WipGood");
        PlayMusicIntro("MainTheme");
    }

    public static void PlayMusicIntro(string trackName)
    {
        Debug.LogWarning("PlayMusicIntro " + trackName);

        Sound s = Array.Find(Instance.musicIntroSounds, sound => sound.name == trackName);

        Assert.IsNotNull(s, "Music Intro" + trackName + " not found");

        if (s != null && !musicIntroSource.isPlaying)
        {
            // Assign new music clip
            musicIntroSource.clip = s.clip;

            musicIntroSource.PlayOneShot(musicIntroSource.clip, musicIntroSource.volume);
            musicIntroClipLength = musicIntroSource.clip.length;
            currentTheme = trackName;

            // Play the intro once and then loop the music
            LoopMusicCaller(trackName, musicIntroClipLength, true);
        }
    }

    public static void PlayMusic(string trackName, bool fadeIn)
    {
        Debug.LogWarning("PlayMusic " + trackName);

        Sound s = Array.Find(Instance.musicSounds, sound => sound.name == trackName);

        Assert.IsNotNull(s, "Music " + trackName + " not found");

        if (s != null && !musicSource.isPlaying)
        {
            // Assign new music clip
            musicSource.clip = s.clip;

            musicSource.PlayOneShot(musicSource.clip, musicSource.volume);
            musicClipLength = musicSource.clip.length;
            currentTheme = trackName;

            if (musicSource.loop)
            {
                if (fadeIn)
                {
                    LoopMusicCaller(trackName, musicClipLength, false);
                }
                else
                {
                    LoopMusicCaller(trackName, musicClipLength, false);
                }
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

    public static IEnumerator LoopMusic(string trackName, float clipLength, bool fadeIn)
    {
        Debug.LogWarning("Loop Music - Begin Waiting " + trackName + " for " + clipLength + " seconds");
        yield return new WaitForSeconds(Mathf.Round(clipLength + 0.05f));
        Debug.LogWarning("Loop Music - Finished Waiting " + trackName);

        PlayMusic(trackName, fadeIn);
    }

    public static IEnumerator FadeInMusic(string trackName, float speed, float maxVolume)
    {
        keepFadingIn = true;
        keepFadingOut = false;

        musicSource.volume = 0;
        float audioVolume = musicSource.volume;

        while (keepFadingIn && musicSource.volume < maxVolume)
        {
            audioVolume += speed;
            musicSource.volume = audioVolume;
            yield return new WaitForSeconds(0.1f);
        }

        musicSource.volume = maxVolume;
    }

    public static IEnumerator FadeOutMusic(string trackName, float speed, float minVolume)
    {
        keepFadingIn = false;
        keepFadingOut = true;

        float audioVolume = musicSource.volume;

        while (keepFadingOut && musicSource.volume > minVolume)
        {
            audioVolume -= speed;
            musicSource.volume = audioVolume;
            yield return new WaitForSeconds(0.1f);
        }

        musicSource.volume = minVolume;
    }

    public static void FadeInCaller(string trackName, float speed, float maxVolume)
    {
        Instance.StartCoroutine(FadeInMusic(trackName, speed, maxVolume));
    }

    public static void FadeOutCaller(string trackName, float speed, float minVolume)
    {
        Instance.StartCoroutine(FadeOutMusic(trackName, speed, minVolume));
    }

    public static void LoopMusicCaller(string trackName, float clipLength, bool fadeIn)
    {
        Instance.StartCoroutine(LoopMusic(trackName, clipLength, fadeIn));
    }
}
