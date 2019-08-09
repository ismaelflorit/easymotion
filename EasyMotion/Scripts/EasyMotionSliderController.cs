/**
 * EasyMotion Plugin
 * Author: Ismael Florit
 * Student Number: 40009944 * 
 * 
 * When assigned to a Slider, saves and loads values.
 * Typically used to control pitch or roll multipliers.
 */

using UnityEngine;
using UnityEngine.UI;

public class EasyMotionSliderController : MonoBehaviour
{
    private Slider slider;
    [SerializeField]
    private string savePath;

    private void OnEnable()
    {
        slider = GetComponent<Slider>();
        slider.value = EasyMotionUtility.LoadFloat(savePath);
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
        EasyMotionUtility.SaveFloat(slider.value, savePath);
    }
}
