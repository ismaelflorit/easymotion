// Borrowed from Unity Standard Assets asset.

using UnityEngine;
using UnityEngine.UI;

public class ToggleAnimation : MonoBehaviour
{ 
    public Toggle toggle;
    public Image toggleBackgroundOff;
    public Image toggleBackgroundOn;
    public Image toggleOff;
    public Image toggleOn;
    public Image labelOn;
    public Image labelOff;

    private void Update()
    {
        MapToggleBackground();
        MapToggle();
        MapLabels();
    }   

    private void MapToggleBackground()
    {
        if (toggle.isOn)
        {
            toggleBackgroundOff.enabled = false;
            toggleBackgroundOn.enabled = true;
        }
        else
        {
            toggleBackgroundOff.enabled = true;
            toggleBackgroundOn.enabled = false;
        }
    }

    private void MapToggle()
    {
        if (toggle.isOn)
        {
            toggleOff.enabled = false;
            toggleOn.enabled = true;
        }
        else
        {
            toggleOff.enabled = true;
            toggleOn.enabled = false;
        }
    }


    private void MapLabels()
    {
        if (toggle.isOn)
        {
            labelOff.enabled = false;
            labelOn.enabled = true;
        }
        else
        {
            labelOff.enabled = true;
            labelOn.enabled = false;
        }
    }

    public void Toggle()
    {
        toggle.isOn = !toggle.isOn;
    }

}