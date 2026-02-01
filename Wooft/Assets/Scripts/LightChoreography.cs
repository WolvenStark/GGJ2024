using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightChoreography : MonoBehaviour
{
    public static LightChoreography Instance;

    protected Light2D GlobalLight;
    protected List<Light2D> GoldenAuras = new List<Light2D>();
    protected List<Light2D> AmbientWindowLighting = new List<Light2D>();

    Coroutine colourShiftRoutine = null;
    public List<Color> WorldColors;

    public float CurrentAuraIntensity = 0.0f;

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
        for (var childId = 0; childId < transform.childCount; childId++)
        {
            var child = transform.GetChild(childId);
            var childName = child.name;

            if (child.name == "Global Light")
            {
                GlobalLight = child.GetComponent<Light2D>();
            }
            else if (child.name == "Golden Aura")
            {
                GoldenAuras.Add(child.GetComponent<Light2D>());
            }
            else if (child.name.Contains("GodRays"))
            {
                AmbientWindowLighting.Add(child.GetComponent<Light2D>());
            }

        }
    }

    public void SetIntensityOfAuraLight(float value)
    {
        CurrentAuraIntensity = value;

        foreach (var light in GoldenAuras)
        {
            light.intensity = value;
        }
    }

    public void SetIntensityOfAmbientLights(float value)
    {
        foreach (var light in AmbientWindowLighting)
        {
            light.intensity = value;
        }
    }

    public void SetWorldColourModeShift(int worldIndex)
    {
        if (colourShiftRoutine != null)
        {
            StopCoroutine(colourShiftRoutine);
            colourShiftRoutine = null;
        }
        StartCoroutine(WaitForColourShift(WorldColors[worldIndex]));
    }

    protected IEnumerator WaitForColourShift(Color to, float duration = 1.0f)
    {
        float elapsed = 0f;
        Color from = GlobalLight.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            Color current = Color.Lerp(from, to, t);

            GlobalLight.color = current;

            yield return null;
        }

        colourShiftRoutine = null;
    }





}
