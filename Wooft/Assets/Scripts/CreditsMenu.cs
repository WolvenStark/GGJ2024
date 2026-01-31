using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsMenu : MonoBehaviour
{
    public static CreditsMenu Instance;

    public bool showCreditsToggle = true;
    protected GameObject UIObject;

    public static event Action<bool> OnToggleUpdate;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            UIObject = gameObject.transform.GetChild(0).gameObject;
        }
        else
        {
            Destroy(gameObject);
        }
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
        }

        OnToggleUpdate.Invoke(showCreditsToggle);
    }
}
