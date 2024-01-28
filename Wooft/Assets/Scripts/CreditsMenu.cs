using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsMenu : MonoBehaviour
{
    public bool showCreditsToggle = true;
    protected GameObject UIObject;

    public string lastMusicTrack = "MainTheme";
    public string menuThemeTrack = "Menu";
    protected bool hasPlayedIntro = false;

    public void Awake()
    {
        UIObject = gameObject.transform.GetChild(1).gameObject;
    }

    protected void Start()
    {
        AudioManager.PlayMusicIntro(menuThemeTrack);
    }

    public void Update()
    {
        if (ToggleDisplayInput())
        {
            ToggleDisplay();
        }

        UpdateDisplay();
    }

    public bool ToggleDisplayInput()
    {
        return Input.GetKeyDown(KeyCode.Tab);
    }

    public void ToggleDisplay()
    {
        showCreditsToggle = !showCreditsToggle;
    }

    public void UpdateDisplay()
    {
        if (UIObject.activeInHierarchy != showCreditsToggle)
        {
            UIObject.SetActive(showCreditsToggle);

            if (showCreditsToggle)
            {
                lastMusicTrack = AudioManager.currentTheme;
                AudioManager.ChangeMusicCaller(menuThemeTrack);
                hasPlayedIntro = false;
            }
            else
            {
                // Restore music

                // First time leaving menu
                if (lastMusicTrack == menuThemeTrack)
                {
                    lastMusicTrack = AudioManager.mainThemeTrack;
                    //Debug.Log("Override intiial music to " + lastMusicTrack);
                }

                AudioManager.StopAllMusic();

                if (hasPlayedIntro)
                {
                    //Debug.Log("Play as is " + lastMusicTrack);

                    AudioManager.ChangeMusicCaller(lastMusicTrack);
                }
                else
                {
                    //Debug.Log("Play with intro " + lastMusicTrack);

                    AudioManager.PlayMusicIntro(lastMusicTrack);
                    AudioManager.ChangeMusicCaller(lastMusicTrack);

                    hasPlayedIntro = true;
                }
            }
        }
    }
}
