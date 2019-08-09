/**
 * EasyMotion Plugin
 * Author: Ismael Florit
 * Student Number: 40009944 * 
 * 
 */

using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class EasyMotionUtility
{
    public static float ClampValueSymmetrically(float value, float limit)
    {
        float clampedValue = value;
        if (clampedValue > limit)
        {
            clampedValue = limit;
        }
        if (clampedValue < -limit)
        {
            clampedValue = -limit;
        }
        return clampedValue;
    }

    public static int ClampValueSymmetrically(int value, int limit)
    {
        int clampedValue = value;

        if (clampedValue > limit)
        {
            clampedValue = limit;
        }
        if (clampedValue < -limit)
        {
            clampedValue = -limit;
        }
        return clampedValue;
    }

    public static void SaveString(string stringValue, string savePath)
    {
        Save(stringValue, savePath);
    }

    public static string LoadString(string savePath)
    {
        return (string)Load(savePath);
    }

    public static void SaveFloat(float floatValue, string savePath)
    {
        Save(floatValue, savePath);
    }

    public static float LoadFloat(string savePath)
    {
        float floatValue = (float)Load(savePath);
        return floatValue;
    }

    public static void SaveBoolean(bool boolValue, string savePath)
    {
        Save(boolValue, savePath);
    }

    public static bool LoadBoolean(string savePath)
    {
        bool boolValue = (bool)Load(savePath);
        return boolValue;
    }

    private static void Save(object value, string savePath)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        using (var fileStream = File.Create(savePath))
        {
            binaryFormatter.Serialize(fileStream, value);
        }
    }

    private static object Load(string savePath)
    {
        object value = null;
        if (File.Exists(savePath))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using(var fileStream = File.Open(savePath, FileMode.Open))
            {
                value = binaryFormatter.Deserialize(fileStream);
            }
        }
        return value;
    }

    public static List<Dropdown.OptionData> GetDropdownOptionsFromStringArray(string[] stringArray)
    {
        List<Dropdown.OptionData> dropdownOptions = new List<Dropdown.OptionData>();
        foreach (string s in stringArray){
            Dropdown.OptionData optionData = new Dropdown.OptionData();
            optionData.text = s;
            dropdownOptions.Add(optionData);
        }
        return dropdownOptions;
    }

    public static void ReOpen<T>(string title, string description) where T : EditorWindow
    {
        T window = (T)EditorWindow.GetWindow(typeof(T));
        window.Close();
        window = (T)EditorWindow.GetWindow(typeof(T));
        window.Show();
        window.titleContent = new GUIContent(title, description);
        window.minSize = new Vector2(300, 400);
        window.maxSize = new Vector2(300, 900);
    }

    public static EasyMotionToggleController[] FindActiveEnabledToggleControllers()
    {
        EasyMotionToggleController[] easyMotionToggleControllers = Resources.FindObjectsOfTypeAll<EasyMotionToggleController>();
        List<EasyMotionToggleController> toggleControllerList = new List<EasyMotionToggleController>();
        List<EasyMotionToggleController> toggleControllersEnabled = new List<EasyMotionToggleController>();
        foreach (EasyMotionToggleController toggleController in easyMotionToggleControllers)
        {
            toggleControllerList.Add(toggleController);
        }
        foreach (EasyMotionToggleController toggleController in toggleControllerList)
        {
            if (toggleController.isActiveAndEnabled)
            {
                toggleControllersEnabled.Add(toggleController);
            }
        }
        return toggleControllersEnabled.ToArray();
    }

    public static EasyMotion[] FindActiveEnabledEasyMotions()
    {
        EasyMotion[] easyMotions = Resources.FindObjectsOfTypeAll<EasyMotion>();
        List<EasyMotion> easyMotionList = new List<EasyMotion>();
        List<EasyMotion> easyMotionsEnabled = new List<EasyMotion>();
        foreach (EasyMotion easyMotion in easyMotions)
        {
            easyMotionList.Add(easyMotion);
        }
        foreach (EasyMotion easyMotion in easyMotionList)
        {
            if (easyMotion.isActiveAndEnabled)
            {
                easyMotionsEnabled.Add(easyMotion);
            }
        }
        return easyMotionsEnabled.ToArray();
    }

    public static EasyMotionSliderController[] FindActiveEnabledSliderControllers()
    {
        EasyMotionSliderController[] sliderControllers = Resources.FindObjectsOfTypeAll<EasyMotionSliderController>();
        List<EasyMotionSliderController> sliderControllerList = new List<EasyMotionSliderController>();
        List<EasyMotionSliderController> sliderControllersEnabled = new List<EasyMotionSliderController>();
        foreach (EasyMotionSliderController dropdownController in sliderControllers)
        {
            sliderControllerList.Add(dropdownController);
        }
        foreach (EasyMotionSliderController sliderController in sliderControllerList)
        {
            if (sliderController.isActiveAndEnabled)
            {
                sliderControllersEnabled.Add(sliderController);
            }
        }
        return sliderControllersEnabled.ToArray();
    }

    public static EasyMotionSerialPortDropdownController[] FindActiveEnabledDropdownControllers()
    {
        EasyMotionSerialPortDropdownController[] dropdownControllers = Resources.FindObjectsOfTypeAll<EasyMotionSerialPortDropdownController>();
        List<EasyMotionSerialPortDropdownController> dropdownControllerList = new List<EasyMotionSerialPortDropdownController>();
        List<EasyMotionSerialPortDropdownController> dropdownControllersEnabled = new List<EasyMotionSerialPortDropdownController>();
        foreach (EasyMotionSerialPortDropdownController dropdownController in dropdownControllers)
        {
            dropdownControllerList.Add(dropdownController);
        }
        foreach (EasyMotionSerialPortDropdownController dropdownController in dropdownControllerList)
        {
            if (dropdownController.isActiveAndEnabled)
            {
                dropdownControllersEnabled.Add(dropdownController);
            }
        }
        return dropdownControllersEnabled.ToArray();
    }

    public static Toggle[] FindActiveToggles()
    {
        Toggle[] availableToggles = Resources.FindObjectsOfTypeAll<Toggle>();
        List<Toggle> toggleList = new List<Toggle>();
        foreach (Toggle toggle in availableToggles)
        {
            if (toggle.gameObject.activeInHierarchy)
            {
                toggleList.Add(toggle);
            }
        }
        return toggleList.ToArray();
    }

    public static Rigidbody[] FindActiveRigidbodies()
    {
        Rigidbody[] availableRigidBodies = Resources.FindObjectsOfTypeAll<Rigidbody>();
        List<Rigidbody> rigidBodyList = new List<Rigidbody>();
        foreach (Rigidbody rigidbody in availableRigidBodies)
        {
            if (rigidbody.gameObject.activeInHierarchy)
            {
                rigidBodyList.Add(rigidbody);
            }
        }
        return rigidBodyList.ToArray();
    }

    public static Dropdown[] FindActiveDropdowns()
    {
        Dropdown[] availableDropdowns = Resources.FindObjectsOfTypeAll<Dropdown>();
        List<Dropdown> dropdownList = new List<Dropdown>();
        foreach (Dropdown dropdown in availableDropdowns)
        {
            if (dropdown.gameObject.activeInHierarchy)
            {
                dropdownList.Add(dropdown);
            }
        }
        return dropdownList.ToArray();
    }
}


