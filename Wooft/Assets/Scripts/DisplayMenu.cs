using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayMenu : MonoBehaviour
{
    public static DisplayMenu Instance;

    public void OnEnable()
    {
        CreditsMenu.OnToggleUpdate += TogglepPauseDisplay;
    }

    public void OnDisable()
    {
        CreditsMenu.OnToggleUpdate -= TogglepPauseDisplay;
    }


    public void Awake()
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

    public void TogglepPauseDisplay(bool toggle)
    {
        if (DialogueController.Instance.isPaused == toggle)
        {
            DialogueController.Instance.isPaused = !toggle;
        }
    }
}
