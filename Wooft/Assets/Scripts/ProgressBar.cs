using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
[RequireComponent(typeof(AudioSource))]
public class ProgressBar : MonoBehaviour
{
    public static ProgressBar Instance;

    protected Slider progress;
    protected ParticleSystem particleEffect;
    protected AudioSource source = null;
    public AudioClip incrementSound;
    public AudioClip decrementSound;

    public float startingProgress = 0.75f;
    public float targetProgress = 0.0f;
    public float currentFillSpeed = 0.4f;
    public const float maxFillSpeed = 0.4f;
    public float errorAllowance = 0.01f;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            source = gameObject.AddComponent<AudioSource>();

            progress = GetComponent<Slider>();
            particleEffect = progress.GetComponentInChildren<ParticleSystem>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        IncrementProgress(startingProgress);
    }

    public void Update()
    {
        if (progress.value < targetProgress)
        {
            //var step = Mathf.Clamp((targetProgress - progress.value), 0, maxFillSpeed);
            progress.value += currentFillSpeed * Time.deltaTime;

            if (!particleEffect.isPlaying)
            {
                particleEffect.Play();
            }
        }
        //else if (progress.value > targetProgress && progress.value > progress.minValue)
        //{
        //    var step = Mathf.Clamp((progress.value - targetProgress), 0, maxFillSpeed);
        //    progress.value -= step * Time.deltaTime;

        //    particleEffect.Stop();
        //}
        else
        {
            //progress.value = targetProgress;
            particleEffect.Stop();
        }

    }

    public void IncrementProgress(float newProgress, float speed = maxFillSpeed)
    {
        targetProgress = progress.value + newProgress;
        currentFillSpeed = speed;

        source.PlayOneShot(incrementSound);
    }

    public void DecrementProgress(float newProgress, float speed = maxFillSpeed)
    {
        targetProgress = progress.value - newProgress;
        currentFillSpeed = speed;
        source.PlayOneShot(decrementSound);
    }
}
