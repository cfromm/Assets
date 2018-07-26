using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StimulusPositionUpdate : MonoBehaviour {
    SMI.SMIEyeTrackingUnity smiInstance = null;
    Vector3 gazeRaycast;
    public Vector3 cyclopeanGazeDirection;
    public Vector3 cyclopeanBasePoint; 
	Quaternion offsets;
    public Vector3 sizeinUnityUnits;
	
    // Use this for initialization
    void Start() {
        smiInstance = SMI.SMIEyeTrackingUnity.Instance;
		offsets = Quaternion.Euler(Experiment.X_offset, Experiment.Y_offset, 0);

        //cameraRaycast = smiInstance.transform.rotation * offsets;
        gazeRaycast =  smiInstance.transform.rotation * offsets * (smiInstance.smi_GetLeftGazeDirection() + smiInstance.smi_GetRightGazeDirection()) * 0.5f + (smiInstance.smi_GetLeftGazeBase() + smiInstance.smi_GetRightGazeBase()) * 0.5f;
		if( !float.IsNaN(gazeRaycast.x) && !float.IsNaN(gazeRaycast.y) && !float.IsNaN(gazeRaycast.z) && Stimulus.GazeContingent ){
			transform.position = smiInstance.transform.position + gazeRaycast * 10;
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
        gazeRaycast = smiInstance.transform.rotation * offsets * cyclopeanGazeDirection + cyclopeanBasePoint;
        //cameraRaycast =  smiInstance.transform.forward;
        if ( !float.IsNaN(gazeRaycast.x) && !float.IsNaN(gazeRaycast.y) && !float.IsNaN(gazeRaycast.z) ){
            transform.position = smiInstance.transform.position + gazeRaycast * Stimulus.StimDepth; //scales magnitude of position by desired value
            //transform.position = new Vector3 (smiInstance.transform.position.x + Stimulus.StimDepth, smiInstance.transform.position.y , smiInstance.transform.position.z );
            //transform.rotation = smiInstance.transform.rotation;
            transform.localScale = new Vector3(2*Mathf.Tan((Stimulus.ApertureRad*Mathf.PI)/180) * Stimulus.StimDepth, 0, 2*Mathf.Tan(Stimulus.ApertureRad * Mathf.PI / 180) * Stimulus.StimDepth);
            //Debug.Log("Local Scale is: " + transform.localScale);
        }
		
	}

    void Update () { 
		
        //set up size conversions to degrees here
        if (Stimulus.GazeContingent)
        { UpdateWithGazePosition(); }

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
