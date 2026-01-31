using FMOD.Studio;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;

public class SFXController : MonoBehaviour
{
    public const string SFXSubPath = "/SFX";
    public const string UISfxSubPath = "/UI";

    public const string ConversationPath = "/Conversation";
    public const string MaskPath = "/Mask";

    EventInstance mainSFXInstance;
    EventInstance mainUISFXInstance;

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

    public void Start()
    {
        mainSFXInstance = RuntimeManager.CreateInstance(AudioManager.AudioPathFormat + SFXSubPath + ConversationPath + ConversationSFXEvent.PlayerVocalisation.ToString());
        mainUISFXInstance = RuntimeManager.CreateInstance(AudioManager.AudioPathFormat + UISfxSubPath + UISFXEvent.ConfirmClick.ToString());
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
