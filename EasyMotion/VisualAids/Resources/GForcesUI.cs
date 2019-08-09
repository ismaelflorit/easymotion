/**
 * EasyMotion Plugin
 * Author: Ismael Florit
 * Student Number: 40009944 * 
 * 
 * Control the GForce visual aid dot.
 */


using UnityEngine;
using UnityEngine.UI;

public class GForcesUI : MonoBehaviour
{
    private RectTransform dot;
    private EasyMotion easyMotion;
    private RawImage dotImage;
    private DotAxes dotAxes;

    private void Start()
    {
        dotAxes = GetComponentInParent<DotAxes>();
        dot = GetComponent<RectTransform>();
        easyMotion = FindObjectOfType<EasyMotion>();
        dotImage = GetComponent<RawImage>();
    }

    private void Update()
    {
        if (easyMotion != null)
        {
            MapGForceDot();
        }
    }

    private void MapGForceDot()
    {
        float longitudinalGForce = easyMotion.GetAverageLongitudinalAcceleration() * 20f;
        float lateralGForce = -easyMotion.GetLateralAcceleration() * 20f;
        float[] limitedAxes = GetLimitedAxesFromHypotenuse(lateralGForce, longitudinalGForce, 95);
        if (!float.IsNaN(dotAxes.x + lateralGForce))
        {
            dot.position = new Vector3(dotAxes.x + limitedAxes[0], dotAxes.y - limitedAxes[1], 1);
        }
        MapGForceDotColorIntensity();
    }

    private float GetHypotenuse(float x, float y)
    {
        return Mathf.Sqrt((x * x) + (y * y));
    }

    /*
     * Keeps dot within circular boundary. If the resultying hypotenuse of x+y exceeds the radius, reassign values accordingly. 
     */
    private float[] GetLimitedAxesFromHypotenuse(float x, float y, float hypotenuseLimit)
    {
        float[] limitedAxes = new float[2];
        if (GetHypotenuse(x, y) > hypotenuseLimit)
        {
            float radiusAngle = RadiusAngle(x, y);
            float yHypotenuseAngle = 90 - radiusAngle;
            x = Mathf.Sin(yHypotenuseAngle * Mathf.Deg2Rad) * hypotenuseLimit;
            y = Mathf.Sin(radiusAngle * Mathf.Deg2Rad) * hypotenuseLimit;
        }
        limitedAxes[0] = x;
        limitedAxes[1] = y;
        return limitedAxes;
    }

    private float RadiusAngle(float x, float y)
    {
        return Mathf.Atan2(y, x) * Mathf.Rad2Deg;
    }

    void MapGForceDotColorIntensity()
    {
        float longitudinalGForce = easyMotion.GetAverageLongitudinalAcceleration();
        float lateralGForce = easyMotion.GetLateralAcceleration();
        dotImage.color = new Color(
            1,
            1 - ((Mathf.Abs(lateralGForce) / 25) + (Mathf.Abs(longitudinalGForce) / 25)),
            1 - ((Mathf.Abs(lateralGForce) / 25) + (Mathf.Abs(longitudinalGForce) / 25)),
            1);
    }
}