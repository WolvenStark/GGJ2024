using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public static MusicController Instance;

    EventInstance musicInstance;

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
        musicInstance = RuntimeManager.CreateInstance("event:/Music/GameMusic");
        musicInstance.start();
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

    public void SetMusicIntensity(float value = 1.0f)
    {
        musicInstance.setParameterByName("Intensity", value);
    }

    public void TestOneShot()
    {
        RuntimeManager.PlayOneShot("event:/UI/ConfirmClick");
    }
}
