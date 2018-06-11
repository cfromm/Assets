using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StimulusPositionUpdate : MonoBehaviour {
    SMI.SMIEyeTrackingUnity smiInstance = null;
    Vector3 cameraRaycast;
	Vector3 offsets;
    public Vector3 sizeinUnityUnits;
	
    // Use this for initialization
    void Start() {
        smiInstance = SMI.SMIEyeTrackingUnity.Instance;
		offsets = new Vector3 (0f, 0f, 0f);
		
		cameraRaycast = smiInstance.transform.rotation * smiInstance.smi_GetCameraRaycast();
        
		offsets.x = Experiment.X_offset;
		offsets.y =	Experiment.Y_offset;
		offsets.z = Experiment.Z_offset;
		if( !float.IsNaN(cameraRaycast.x) && !float.IsNaN(cameraRaycast.y) && !float.IsNaN(cameraRaycast.z) ){
			transform.position = smiInstance.transform.position + cameraRaycast * 10;
		}
    }


    /// <summary>
    /// Get the SMI gaze position and add the desired offset
    /// To be called in update if gaze-contingent stimulus is desired
    /// </summary>
    void UpdateWithGazePosition()
    {
        cameraRaycast = smiInstance.transform.rotation * smiInstance.smi_GetCameraRaycast();
        
		if( !float.IsNaN(cameraRaycast.x) && !float.IsNaN(cameraRaycast.y) && !float.IsNaN(cameraRaycast.z) ){
            transform.position = smiInstance.transform.position + cameraRaycast * Stimulus.StimDepth; //scales magnitude of position by desired value
            // replace Stimulus.StimDepth with cameraRaycast.y if you want to use binocular depth cue from SMI
            Debug.Log("Stimulus is " + Mathf.Sin(Stimulus.ApertureRad * Mathf.PI / 180f) + " meters big, viewed at " +Stimulus.StimDepth);
            transform.localScale = new Vector3(2*Mathf.Sin((Stimulus.ApertureRad*Mathf.PI)/180) * Stimulus.StimDepth, 0, 2*Mathf.Sin(Stimulus.ApertureRad * Mathf.PI / 180) * Stimulus.StimDepth);
            //CHECK CONVERSION FROM RADIANS TO DEGREES IN THE SIN FUNCTION
        }
		
	}

	
    // Update is called once per frame
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
