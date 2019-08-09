/**
 * EasyMotion Plugin
 * Author: Ismael Florit
 * Student Number: 40009944
 * 
 * When assigned to a Dropdown, populates and maps SerialPort COM ports for use in a menu.
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.IO.Ports;

public class EasyMotionSerialPortDropdownController : MonoBehaviour
{
    private Dropdown dropdown;
    private List<Dropdown.OptionData> dropdownOptions = new List<Dropdown.OptionData>();
    string[] availablePorts;
    private string savePath;
    private int loadedOptionValue;

    void Start()
    {
        savePath = Application.persistentDataPath + EasyMotionConstants.serialPortSavePath;
        dropdown = GetComponent<Dropdown>();
        availablePorts = SerialPort.GetPortNames();
        SetPortOptions(availablePorts);
        SetLoadedOption();
    }

    private void SetPortOptions(string[] ports)
    {
        dropdown.options = EasyMotionUtility.GetDropdownOptionsFromStringArray(ports);
    }

    private void OnDisable()
    {
        string selectedDropdown = availablePorts[dropdown.value];
        EasyMotionUtility.SaveString(selectedDropdown, savePath);
    }

    private int GetLoadedOptionValue()
    {
        int value = 0;
        if (File.Exists(savePath))
        {
            string loadedOption = EasyMotionUtility.LoadString(savePath);
            for(int i = 0; i < availablePorts.Length; i++)
            {
                if (availablePorts[i].Equals(loadedOption))
                {
                    value = i;
                }
            }
        }
        return value;
    }

    private void SetLoadedOption()
    {
        dropdown.value = GetLoadedOptionValue();
    }
}