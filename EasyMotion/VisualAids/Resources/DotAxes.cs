/**
 * EasyMotion Plugin
 * Author: Ismael Florit
 * Student Number: 40009944 * 
 * 
 * Pivot script for GForcesUI
 */

using UnityEngine;

public class DotAxes : MonoBehaviour {

    public float x;
    public float y;
	
	void Update ()
    {
        x = transform.position.x;
        y = transform.position.y;
	}
}
