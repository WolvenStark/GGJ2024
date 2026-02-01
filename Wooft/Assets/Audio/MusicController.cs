using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public static MusicController Instance;

    protected EventInstance musicInstance;
    protected MusicState currentMusicState;

    public const string MusicSubPath = "/Music";
    public const string MainThemePath = "/GameMusic";

    public enum MusicState
    { 
        PartOnePreKing = 0,
        PartTwoPostKing,
        ConclusionDed,
        ConclusionMask,
        ConclusionEscape,
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
        musicInstance = RuntimeManager.CreateInstance(AudioManager.AudioPathFormat + MusicSubPath + MainThemePath);
        musicInstance.start();
        //SetMusicState(MusicState.PartTwoPostKing);
    }

    public void OnDestroy()
    {
        StopMusic();
        musicInstance.release();
    }

    public void StopMusic()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void PauseMusic()
    {
        musicInstance.setPaused(true);
    }

    public void SetMusicState(MusicState state)
    {
        if (state == currentMusicState)
        {
            return;
        }

        currentMusicState = state;
        musicInstance.setParameterByName("GameMusicController", (float)state);
    }

    public void SetMusicIntensity(float value = 1.0f)
    {
        musicInstance.setParameterByName("Intensity", value);
    }

    public void TestOneShot()
    {
        RuntimeManager.PlayOneShot("event:/UI/ConfirmClick");
    }
}
