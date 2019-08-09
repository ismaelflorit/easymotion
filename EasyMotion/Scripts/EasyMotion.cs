/**
 * EasyMotion Plugin
 * Author: Ismael Florit
 * Student Number: 40009944
 * 
 * Contains simple motion cueing algorithms for use with any Rigidbody rigged vehicle.
 * Current Arduino firmware supported: SMC3
 */

using UnityEngine;
using System.IO.Ports;
using System;

public class EasyMotion : MonoBehaviour
{
    enum FirmwareOptions { SMC3 };
    private SerialPort port = new SerialPort();
    [SerializeField] private float developerLongitudonalMotionMultiplier = 10;
    [SerializeField] private float developerLateralMotionMultiplier = 40;
    public float playerLongitudonalMotionMultipler = 1f;
    public float playerLateralMotionMultiplier = 1f;
    private float[] longitudinalAccelerationSampledData = new float[10];
    private int longitudinalSamplesIndex = 0;
    private float lastLongitudinalVelocity = 0;
    private Rigidbody rigidBody;
    private bool isJittering = false;
    public float jitterAmount = 1;
    private float[] jitter = new float[2];
    private int jitterIndex = 0;
    private float platformCentre = 511;
    public float seatPitchModifier = 50;
    private int motorLimiterAmount = 289;
    private bool platformIsEnabled;
    private FirmwareOptions selectedFirmware = FirmwareOptions.SMC3; // currently only one firmware supported.
    public float platformRoll;

#if UNITY_EDITOR
    void Reset()
    {
        InitialiseVisualAidPrefab();
    }

    private void InitialiseVisualAidPrefab()
    {
        if (!GameObject.Find(EasyMotionConstants.visualAidPrefab))
        {
            GameObject visualAidPrefab = (GameObject)Instantiate(Resources.Load(EasyMotionConstants.visualAidPrefab));
            visualAidPrefab.name = EasyMotionConstants.visualAidPrefab;
            visualAidPrefab.hideFlags = HideFlags.HideInHierarchy;
        }
    }

#endif
    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        LoadSerialPortName();
        SetupPort();
        platformIsEnabled = LoadPluginState();
    }

    public float GetPlatformCentre()
    {
        return platformCentre;
    }

    private void SetPlatformCentre(float platformCentre)
    {
        this.platformCentre = platformCentre;
    }

    private void SetupPort()
    {
        port.DtrEnable = true;
        port.BaudRate = 500000;
        try
        {
            port.Open();
        }
        catch (Exception e)
        {
            Debug.LogError("<b>[EasyMotion]</b> Unable to establish communication with the platform : "
                + e.Message);
        }
    }

    private void LoadSerialPortName()
    {
        string savePath = Application.persistentDataPath + EasyMotionConstants.serialPortSavePath;
        try
        {
            port.PortName = EasyMotionUtility.LoadString(savePath);
        }
        catch (Exception e)
        {
            if (e is ArgumentNullException)
            {

                Debug.LogError("<b>[EasyMotion]</b> Port Dropdown reference was <color=blue>null</color>.  Check it exists - Platform is disabled.");
            }
        }
    }

    private bool LoadPluginState()
    {
        bool state = false;
        string savePath = Application.persistentDataPath + EasyMotionConstants.toggleSavePath;
        try
        {
            state = EasyMotionUtility.LoadBoolean(savePath);
        }
        catch (NullReferenceException)
        {
            Debug.LogError("<b>[EasyMotion]</b> On-Off Toggle reference was <color=blue>null</color>.  Check it exists - Platform is disabled.");
        }
        return state;
    }

    private void LoadPlayerRollMotionMultiplier()
    {
        string savePath = Application.persistentDataPath + EasyMotionConstants.rollSliderSavePath;
        playerLateralMotionMultiplier = EasyMotionUtility.LoadFloat(savePath);
    }

    private void LoadPlayerPitchMotionMultiplier()
    {
        string savePath = Application.persistentDataPath + EasyMotionConstants.pitchSliderSavePath;
        playerLongitudonalMotionMultipler = EasyMotionUtility.LoadFloat(savePath);
    }

    private float GetJitter()
    {
        if (jitterAmount > 10)
        {
            jitterAmount = 10;
        }
        jitter[0] = jitterAmount;
        jitter[1] = -jitterAmount;
        if (jitterIndex < (jitter.Length - 1))
        {
            jitterIndex++;
        }
        else
        {
            jitterIndex = 0;
        }
        return jitter[jitterIndex];
    }

    private void FixedUpdate()
    {
        SampleLongitudinalAccelerationData();
        if (platformIsEnabled)
        {
            SendPackets();
        }
        else
        {
            ReturnRoll();
        }
    }

    public float GetLateralAcceleration()
    {
        return GetVelocitySquared() / GetTurningRadius();
    }

    private float GetTurningRadius()
    {
        return GetVelocitySquared() / rigidBody.angularVelocity.y;
    }

    private float GetVelocitySquared()
    {
        Vector3 direction = gameObject.transform.right;
        return Vector3.Dot(rigidBody.velocity, direction);
    }

    private void SampleLongitudinalAccelerationData()
    {
        float acceleration = (rigidBody.velocity.magnitude - lastLongitudinalVelocity) / Time.fixedDeltaTime;
        longitudinalAccelerationSampledData[longitudinalSamplesIndex] = acceleration;
        lastLongitudinalVelocity = rigidBody.velocity.magnitude;
        if (longitudinalSamplesIndex.Equals(longitudinalAccelerationSampledData.Length - 1))
        {
            longitudinalSamplesIndex = 0;
        }
        else
        {
            longitudinalSamplesIndex++;
        }
    }

    public float GetAverageLongitudinalAcceleration()
    {
        float sum = 0;
        foreach (float sample in longitudinalAccelerationSampledData)
        {
            sum += sample;
        }
        float longitudinalAcceleration = sum / longitudinalAccelerationSampledData.Length;
        if (!VehicleIsTravellingForward())
        {
            longitudinalAcceleration = -longitudinalAcceleration;
        }
        return longitudinalAcceleration / 2;
    }

    private float GetPitchRelativeToWorld()
    {
        Vector3 right = transform.right;
        right.y = 0;
        right *= Mathf.Sign(transform.up.y);
        Vector3 forward = Vector3.Cross(right, Vector3.up).normalized;
        float vehiclePitch = Vector3.Angle(forward, transform.forward) * Mathf.Sign(transform.forward.y);
        vehiclePitch = AccountForSpeed(vehiclePitch);
        return vehiclePitch;
    }

    private float GetRollRelativeToWorld()
    {
        Vector3 forward = transform.forward;
        forward.y = 0;
        forward *= Mathf.Sign(transform.up.y);
        Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;
        float vehicleRoll = Vector3.Angle(right, transform.right) * Mathf.Sign(transform.right.y);
        float original = vehicleRoll;
        vehicleRoll = AccountForSpeed(vehicleRoll);
        return vehicleRoll;
    }

    /*
     * Simple washout filter for use with GetPitchRelativeToWorld() and GetRollRelativeToWorld()
     */
    private float AccountForSpeed(float value)
    {
        float factor = 1.0001f;
        float multiplyingFactor = factor / Mathf.Sqrt(rigidBody.velocity.magnitude);
        multiplyingFactor = Mathf.Clamp(multiplyingFactor, 0.0001f, 1);
        return value * multiplyingFactor;
    }

    public float ReturnPitch()
    {
        float longitudinalAcceleration = GetAverageLongitudinalAcceleration();
        float centreOffset = ReturnCentreOffset(platformRoll);
        float vehiclePitch = GetPitchRelativeToWorld();
        float pitch = centreOffset + seatPitchModifier + (developerLongitudonalMotionMultiplier * longitudinalAcceleration) + (vehiclePitch * 5);
        if (rigidBody.velocity.magnitude < 0.25) // prevent unrealistic jolts when stopping.
        {
            pitch = centreOffset + seatPitchModifier + (vehiclePitch * 5);
        }
        return pitch;
    }

    public void ReturnRoll()
    {
        float lateralAcceleration = GetLateralAcceleration();
        float vehicleRoll = GetRollRelativeToWorld();

        float roll = platformCentre + (developerLateralMotionMultiplier * lateralAcceleration) + (vehicleRoll * 5);
        if (isJittering)
        {
            roll += GetJitter();
        }
        platformRoll = roll;
    }

    /*
     * Axes moved parallelly in the platform create roll. ReturnMotorValues() uses this as second parameter.
     */
    private float ReturnCentreOffset(float roll)
    {
        float centreOffset = platformCentre;
        if (roll > platformCentre)
        {
            centreOffset = platformCentre + (roll - platformCentre);
        }
        else
        {
            centreOffset = platformCentre - (platformCentre - roll);
        }
        return centreOffset;
    }

    private void SendPackets()
    {
        ReturnRoll();
        int[] motorValues = ReturnMotorValues(ReturnPitch(), ReturnCentreOffset(platformRoll));
        int[] limitedMotorValues = ReturnLimitedMotorValues(motorValues[0], motorValues[1], (int)platformCentre, motorLimiterAmount);
        byte[] packetToSend = new byte[2];
        switch (selectedFirmware)
        {
            case FirmwareOptions.SMC3:
                packetToSend = SMC3HexadecimalFormatter(limitedMotorValues[0], limitedMotorValues[1]);
                break;
        }
        if (port.IsOpen)
        {
            try
            {
                port.Write(packetToSend, 0, packetToSend.Length);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }

    private byte[] SMC3HexadecimalFormatter(int valueA, int valueB)
    {
        byte[] bothMotors = new byte[10];
        byte[] resetMotors = { 0x5B, 0x41, 0x01, 0xFF, 0x5D, 0x5B, 0x42, 0x01, 0xFF, 0x5D };

        string byte3 = valueA.ToString("X4").Substring(0, 2);
        string byte4 = valueA.ToString("X4").Substring(2, 2);
        string byte9 = valueB.ToString("X4").Substring(0, 2);
        string byte10 = valueB.ToString("X4").Substring(2, 2);

        bothMotors[0] = 0x5B;
        bothMotors[1] = 0x41;
        bothMotors[2] = Convert.ToByte(byte3, 16);
        bothMotors[3] = Convert.ToByte(byte4, 16);
        bothMotors[4] = 0x5D;

        bothMotors[5] = 0x5B;
        bothMotors[6] = 0x42;
        bothMotors[7] = Convert.ToByte(byte9, 16);
        bothMotors[8] = Convert.ToByte(byte10, 16);
        bothMotors[9] = 0x5D;

        return bothMotors;
    }

    public int[] ReturnLimitedMotorValues(int motorA, int motorB, int centreOfPlatform, int limitAmount)
    {
        int upperLimit = centreOfPlatform + limitAmount;
        int lowerLimit = centreOfPlatform - limitAmount;
        int[] limitedMotors = new int[2];
        limitedMotors[0] = motorA;
        limitedMotors[1] = motorB;

        if (motorA > upperLimit)
        {
            limitedMotors[0] = upperLimit;
        }
        if (motorA < lowerLimit)
        {
            limitedMotors[0] = lowerLimit;
        }
        if (motorB > upperLimit)
        {
            limitedMotors[1] = upperLimit;
        }
        if (motorB < lowerLimit)
        {
            limitedMotors[1] = lowerLimit;
        }
        return limitedMotors;
    }

    private int[] ReturnMotorValues(float pitch, float centreOffset)
    {
        int[] motors = new int[2];
        float motorA;
        float motorB;
        if (pitch > platformCentre)
        {
            motorA = centreOffset - (pitch - platformCentre);
            motorB = centreOffset + (pitch - platformCentre);
        }
        else
        {
            motorA = centreOffset + (platformCentre - pitch);
            motorB = centreOffset - (platformCentre - pitch);
        }

        motors[0] = (int)motorA;
        motors[1] = (int)motorB;
        return motors;
    }

    private bool VehicleIsTravellingForward()
    {
        bool vehicleIsTravellingForward;
        if (transform.InverseTransformDirection(rigidBody.velocity).z > 0)
        {
            vehicleIsTravellingForward = true;
        }
        else
        {
            vehicleIsTravellingForward = false;
        }
        return vehicleIsTravellingForward;
    }

    void Update()
    {
        platformRoll += GetJitter(); // roll needs to be global and ran separate to FixedUpdate() otherwise poll is too fast and jitter is not created.
    }

    public void StartJitterEffect(float jitterStrength)
    {
        jitterStrength = EasyMotionUtility.ClampValueSymmetrically(jitterStrength, 10);
        isJittering = true;
        jitterAmount = jitterStrength;
    }

    public void StopJitterEffect()
    {
        isJittering = false;
    }

    private void OnApplicationQuit()
    {
        port.Close();
    }
}