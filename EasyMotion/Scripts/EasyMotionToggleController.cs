/**
 * EasyMotion Plugin
 * Author: Ismael Florit
 * Student Number: 40009944 * 
 * 
 * When assigned to a Toggle, saves and loads a value.
 * Typically used to control the platform state.
 */

using UnityEngine;
using UnityEngine.UI;

public class EasyMotionToggleController : MonoBehaviour
{
    private Toggle toggle;
    [SerializeField]
    private string savePath;

    private void OnEnable()
    {
        toggle = GetComponent<Toggle>();
        toggle.isOn = EasyMotionUtility.LoadBoolean(savePath);
    }

    public void SetSavePath(string savePath)
    {
        this.savePath = Application.persistentDataPath + savePath;
    }

    public string GetSavePath()
    {
        return savePath;
    }

    private void OnDisable()
    {
        EasyMotionUtility.SaveBoolean(toggle.isOn, savePath);
    }
}
