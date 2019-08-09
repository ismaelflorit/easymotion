/**
 * EasyMotion Plugin
 * Author: Ismael Florit
 * Student Number: 40009944 * 
 * 
 * Checks constantly whether EasyMotion is active or not.
 * Disables the visual aids if it is not.
 */
 #if UNITY_EDITOR
using UnityEngine;

[ExecuteAlways]
public class EasyMotionActiveState : MonoBehaviour
{
    EasyMotion[] easyMotions;

    private void Start()
    {
        easyMotions = Resources.FindObjectsOfTypeAll<EasyMotion>();
    }

    private void Update()
    {
        if (!EasyMotionIsActive())
        {
            DisableVisualAidPanels();
        }
        else
        {
            EnableVisualAidPanels();
        }
    }

    private bool EasyMotionIsActive()
    {
        bool active = false;
        if (easyMotions[0].isActiveAndEnabled)
        {
            active = true;
        }
        return active;
    }

    private void DisableVisualAidPanels()
    {
        transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    private void EnableVisualAidPanels()
    {
        transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }
}
#endif
