using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class SFXController : MonoBehaviour
{
    public static SFXController Instance;

    public const string SFXSubPath = "/SFX";
    public const string UISfxSubPath = "/UI";

    public const string ConversationPath = "/Conversation";
    public const string MaskPath = "/Mask";

    EventInstance mainSFXInstance;
    EventInstance mainUISFXInstance;

    public const string pathDivider = "/";

    public enum ConversationSFXEvent
    {
        PlayerVocalisation = 0,
        CourtierVocalisation,
        KingVocalisation,
    }
    public enum MaskSFXEvent
    {
        MaskBreak = 0,
        MaskFuse,
    }

    public enum UISFXEvent
    {
        ConfirmClick = 0,
        Transition, //Pageturn
    }

    protected void Awake()
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

    public void Start()
    { 
        mainSFXInstance = RuntimeManager.CreateInstance(AudioManager.AudioPathFormat + SFXSubPath + ConversationPath + pathDivider + ConversationSFXEvent.PlayerVocalisation.ToString() + " ");
        mainUISFXInstance = RuntimeManager.CreateInstance(AudioManager.AudioPathFormat + UISfxSubPath + pathDivider + UISFXEvent.ConfirmClick.ToString());
    }

    public void PlayConversationSFX(ConversationSFXEvent option)
    {
        string eventPath = AudioManager.AudioPathFormat + SFXSubPath + ConversationPath + pathDivider + option.ToString();
        if (option != ConversationSFXEvent.CourtierVocalisation)
        {
            eventPath += " "; // Found lingering space at end of sound
        }

        PlayOneShot(eventPath);
    }

    public void PlayMaskSFX(MaskSFXEvent option)
    {
        string eventPath = AudioManager.AudioPathFormat + SFXSubPath + MaskPath + pathDivider + option.ToString();
        PlayOneShot(eventPath);
    }

    public void PlayUISFX(UISFXEvent option)
    {
        string eventPath = AudioManager.AudioPathFormat + UISfxSubPath + pathDivider + option.ToString();
        PlayOneShot(eventPath);
    }

    public void PlayOneShot(string eventPath)
    {
        // Create a one-shot instance, start it, and release it immediately
        EventInstance oneShotInstance = RuntimeManager.CreateInstance(eventPath);

        oneShotInstance.start();
        oneShotInstance.release(); // Safe: release AFTER start for one-shots. Tells FMOD to clean up automatically when finished
    }


    public void PlayInterruptibleSFX(string eventPath)
    {
        mainSFXInstance = RuntimeManager.CreateInstance(eventPath);
        mainSFXInstance.start();
    }

    public void StopWithFadeSFX()
    {
        mainSFXInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        mainSFXInstance.release();
    }

    public void PlayInterruptibleUISFX(string eventPath)
    {
        mainUISFXInstance = RuntimeManager.CreateInstance(eventPath);
        mainUISFXInstance.start();
    }

    public void StopWithFadeUISFX()
    {
        mainUISFXInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        mainUISFXInstance.release();
    }
}
