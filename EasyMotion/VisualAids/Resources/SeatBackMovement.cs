/**
 * EasyMotion Plugin
 * Author: Ismael Florit
 * Student Number: 40009944 * 
 * 
 * Controls the rear view rotation of platform simulation.
 */

using UnityEngine;

public class SeatBackMovement : MonoBehaviour
{
    private GameObject vehicle;
    private EasyMotion easyMotion;
    private RectTransform seat;
    [Range(0f, 100f)] public float dataDampening = 8f;
    [Range(0f, 100f)] public float platformRoll = 8f;


    void Start()
    {
        easyMotion = FindObjectOfType<EasyMotion>();
        seat = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (easyMotion != null)
        {
            SeatRotation();
        }
        platformRoll = easyMotion.platformRoll;
    }

    void SeatRotation()
    {
        float roll = easyMotion.platformRoll - easyMotion.GetPlatformCentre();
        float rotationLimit = 10.5f;
        float rotation = roll / dataDampening;
        rotation = EasyMotionUtility.ClampValueSymmetrically(rotation, rotationLimit);
        if (!float.IsNaN(rotation))
        {
            seat.localEulerAngles = new Vector3(0, 0, rotation);
        }
    }
}
