/**
 * EasyMotion Plugin
 * Author: Ismael Florit
 * Student Number: 40009944 * 
 * 
 * Controls the side view rotation of platform simulation.
 */

using UnityEngine;

public class SeatSideMovement : MonoBehaviour {

    private EasyMotion easyMotion;
    private RectTransform seat;

    void Start ()
    {
        easyMotion = FindObjectOfType<EasyMotion>();
        seat = GetComponent<RectTransform>();
	}
	
	void Update ()
    {
        if (easyMotion != null)
        {
            SeatRotation();
        }
    }

    void SeatRotation()
    {
        float dataDampening = 10f;
        float rotationLimit = 9.5f;
        float pitch = easyMotion.ReturnPitch() - easyMotion.seatPitchModifier - easyMotion.GetPlatformCentre();
        float rotation = pitch / dataDampening;
        rotation = -rotation;
        rotation = EasyMotionUtility.ClampValueSymmetrically(rotation, rotationLimit);
        if (!float.IsNaN(rotation))
        {
            seat.localEulerAngles = new Vector3(0, 0, rotation);
        }
    }
}
