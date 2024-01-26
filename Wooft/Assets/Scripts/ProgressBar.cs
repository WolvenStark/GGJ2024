using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
[RequireComponent(typeof(AudioSource))]
public class ProgressBar : MonoBehaviour
{
    protected Slider progress;
    protected ParticleSystem particleEffect;
    protected AudioSource source = null;
    public AudioClip incrementSound;
    public AudioClip decrementSound;

    public float startingProgress = 0.75f;
    public float targetProgress = 0.0f;
    public float maxFillSpeed = 0.4f;
    public float errorAllowance = 0.01f;

    public void Awake()
    {
        source = gameObject.AddComponent<AudioSource>();

        progress = GetComponent<Slider>();
        particleEffect = progress.GetComponentInChildren<ParticleSystem>();
    }

    public void Start()
    {
        IncrementProgress(startingProgress);
    }

    public void Update()
    {
        if (progress.value < (targetProgress - errorAllowance) && progress.value < progress.maxValue)
        {
            var step = Mathf.Clamp((targetProgress - progress.value), 0, maxFillSpeed);
            progress.value += step * Time.deltaTime;

            if (!particleEffect.isPlaying)
            {
                particleEffect.Play();
            }
        }
        else if (progress.value > (targetProgress - errorAllowance) && progress.value > progress.minValue)
        {
            var step = Mathf.Clamp((progress.value - targetProgress), 0, maxFillSpeed);
            progress.value -= step * Time.deltaTime;

            particleEffect.Stop();
        }
        else
        {
            particleEffect.Stop();
        }

    }

    public void IncrementProgress(float newProgress)
    {
        targetProgress = progress.value + newProgress;
        targetProgress = Mathf.Clamp01(targetProgress);

        source.PlayOneShot(incrementSound);
    }

    public void DecrementProgress(float newProgress)
    {
        targetProgress = progress.value - newProgress;
        targetProgress = Mathf.Clamp(targetProgress, progress.minValue, progress.maxValue);

        source.PlayOneShot(decrementSound);
    }
}
