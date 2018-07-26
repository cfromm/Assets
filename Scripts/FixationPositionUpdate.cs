using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixationPositionUpdate : MonoBehaviour {

    SMI.SMIEyeTrackingUnity smiInstance = null;
    Vector3 fixationRaycast;
    Quaternion offsets;
    public Vector3 sizeinUnityUnits;
    public Vector3 displacementVector;
    public Vector3 cyclopeanBasePoint;
    public Vector3 cyclopeanGazeDirection;
    public Vector3 gazeVector;
    public float angularError;


    // Use this for initialization
    void Start()
    {
        smiInstance = SMI.SMIEyeTrackingUnity.Instance;
        fixationRaycast = smiInstance.transform.forward;
        if (!float.IsNaN(fixationRaycast.x) && !float.IsNaN(fixationRaycast.y) && !float.IsNaN(fixationRaycast.z) && Stimulus.GazeContingent)
        {
            transform.position = smiInstance.transform.position + fixationRaycast * 10;
        }
    }


    /// <summary>
    /// Get the SMI gaze position and add the desired offset
    /// To be called in update if gaze-contingent stimulus is desired
    /// </summary>
    void UpdateWithGazePosition()
    {
        cyclopeanBasePoint = (smiInstance.smi_GetLeftGazeBase() + smiInstance.smi_GetRightGazeBase()) * 0.5f;
        cyclopeanGazeDirection = (smiInstance.smi_GetLeftGazeDirection() + smiInstance.smi_GetRightGazeDirection()) * 0.5f;
        fixationRaycast = smiInstance.transform.forward + cyclopeanBasePoint;
        if (!float.IsNaN(fixationRaycast.x) && !float.IsNaN(fixationRaycast.y) && !float.IsNaN(fixationRaycast.z))
        {
            gazeVector = smiInstance.transform.position + cyclopeanBasePoint + smiInstance.transform.rotation * offsets * cyclopeanGazeDirection * Stimulus.StimDepth ;
            transform.position = smiInstance.transform.position + fixationRaycast * Stimulus.StimDepth;
            displacementVector = gazeVector - transform.position;//scales magnitude of position by desired value
            transform.localScale = new Vector3(2 * Mathf.Tan((Experiment.FixationRad * Mathf.PI) / 180) * Stimulus.StimDepth, 0, 2 * Mathf.Tan(Experiment.FixationRad * Mathf.PI / 180) * Stimulus.StimDepth);
        }

    }

    void Update()
    {
       // angularError = smiInstance.smi_GetCameraRaycast() * Stimulus.StimDepth - transform.position;
        //set up size conversions to degrees here
        if (Stimulus.GazeContingent)
        { UpdateWithGazePosition();
            angularError = 2*Mathf.Atan(displacementVector.magnitude /2*Stimulus.StimDepth)*180/Mathf.PI;
            Debug.Log(angularError);
        }

        // Make the stimulus facing user
        if (smiInstance != null)
        {
            transform.LookAt(smiInstance.transform);

            if (Stimulus.Type == "t")
            {
                transform.rotation *= Quaternion.Euler(0, 180, 0);
            }
            if (Stimulus.Type == "d")
            {
                transform.rotation *= Quaternion.Euler(90, 0, 0);
            }
        }
    }
}

