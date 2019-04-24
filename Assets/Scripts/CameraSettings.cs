using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSettings : MonoBehaviour {

    void Start()
    {

        if (Experiment.Monocular == "r")
        { Camera.main.stereoTargetEye = StereoTargetEyeMask.Right;
            Debug.Log("Right eye"); }
        if (Experiment.Monocular == "l")

        {
            Camera.main.stereoTargetEye = StereoTargetEyeMask.Left;
            Debug.Log("Left eye");
        }
        if (Experiment.Monocular == "b")
        { Camera.main.stereoTargetEye = StereoTargetEyeMask.Both;

            Debug.Log("Both eyes");
        }
    }
}