using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsMenu : MonoBehaviour
{
    public bool showCreditsToggle = true;
    protected GameObject UIObject;

    public void Awake()
    {
        UIObject = gameObject.transform.GetChild(0).gameObject;
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
    }

}
