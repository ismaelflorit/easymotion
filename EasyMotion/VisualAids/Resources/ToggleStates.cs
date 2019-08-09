/**
 * EasyMotion Plugin
 * Author: Ismael Florit
 * Student Number: 40009944 * 
 * 
 * Monitors and maps toggle states of the EasyMotion editor window.
 */

using UnityEngine;

public class ToggleStates : MonoBehaviour
{
    public bool forceSettingsHeaderGroupToggled;
    public bool visualAidHeaderGroupToggled;
    public bool jitterEffectHeaderGroupToggled;
    public bool platformSimulationToggled;
    public bool gForceSimulationToggled;
    public bool jitterEffectToggled;

    public void DefaultAllTogglesToTrue()
    {
        forceSettingsHeaderGroupToggled = true;
        visualAidHeaderGroupToggled = true;
        jitterEffectHeaderGroupToggled = true;
        platformSimulationToggled = true;
        gForceSimulationToggled = true;
        jitterEffectToggled = true;
        PlayerPrefs.SetInt(EasyMotionConstants.forceSettingsHeaderGroup, 1);
        PlayerPrefs.SetInt(EasyMotionConstants.visualAidHeaderGroup, 1);
        PlayerPrefs.SetInt(EasyMotionConstants.jitterEffectHeaderGroup, 1);
        PlayerPrefs.SetInt(EasyMotionConstants.platformSimulationButton, 1);
        PlayerPrefs.SetInt(EasyMotionConstants.gForceSimulationButton, 1);
        PlayerPrefs.SetInt(EasyMotionConstants.jitterEffectToggled, 1);
    }  

    private void OnApplicationQuit()
    {
        SetHeaderGroupStates();
        SetVisualAidStates();
        SetJitterEffectHeaderGroupState();
        SetJitterEffectState();
    }

    private void SetVisualAidStates()
    {
        SetPlatformSimulationPressedState();
        SetGForceUIPressedState();
    }

    private void SetPlatformSimulationPressedState()
    {
        if (platformSimulationToggled)
        {
            PlayerPrefs.SetInt(EasyMotionConstants.platformSimulationButton, 1);
        }
        else
        {
            PlayerPrefs.SetInt(EasyMotionConstants.platformSimulationButton, 0);
        }
    }

    private void SetGForceUIPressedState()
    {
        if (gForceSimulationToggled)
        {
            PlayerPrefs.SetInt(EasyMotionConstants.gForceSimulationButton, 1);
        }
        else
        {
            PlayerPrefs.SetInt(EasyMotionConstants.gForceSimulationButton, 0);
        }
    }

    private void SetJitterEffectState()
    {
        if (jitterEffectToggled)
        {
            PlayerPrefs.SetInt(EasyMotionConstants.jitterEffectToggled, 1);
        }
        else
        {
            PlayerPrefs.SetInt(EasyMotionConstants.jitterEffectToggled, 0);
        }
    }

    private void SetHeaderGroupStates()
    {
        SetForceHeaderGroupState();
        SetVisualHeaderGroupState();
        SetJitterEffectHeaderGroupState();
    }

    private void SetForceHeaderGroupState()
    {
        if (forceSettingsHeaderGroupToggled)
        {
            PlayerPrefs.SetInt(EasyMotionConstants.forceSettingsHeaderGroup, 1);
        }
        else
        {
            PlayerPrefs.SetInt(EasyMotionConstants.forceSettingsHeaderGroup, 0);
        }
    }

    private void SetVisualHeaderGroupState()
    {
        if (visualAidHeaderGroupToggled)
        {
            PlayerPrefs.SetInt(EasyMotionConstants.visualAidHeaderGroup, 1);
        }
        else
        {
            PlayerPrefs.SetInt(EasyMotionConstants.visualAidHeaderGroup, 0);
        }
    }

    private void SetJitterEffectHeaderGroupState()
    {
        if (jitterEffectHeaderGroupToggled)
        {
            PlayerPrefs.SetInt(EasyMotionConstants.jitterEffectHeaderGroup, 1);
        }
        else
        {
            PlayerPrefs.SetInt(EasyMotionConstants.jitterEffectHeaderGroup, 0);
        }
    }
}