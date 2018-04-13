using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StimulusPositionUpdate : MonoBehaviour {
    SMI.SMIEyeTrackingUnity smiInstance = null;
    Vector3 cameraRaycast;
    Vector3 leftGazeDirection;
    Vector3 rightGazeDirection;
    Vector3 stim_position;
	Vector3 offsets;
	
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
        //leftGazeDirection = smiInstance.transform.rotation * smiInstance.smi_GetLeftGazeDirection();
        //rightGazeDirection = smiInstance.transform.rotation * smiInstance.smi_GetRightGazeDirection();
        cameraRaycast = smiInstance.transform.rotation * smiInstance.smi_GetCameraRaycast();
        //stim_position = (leftGazeDirection + rightGazeDirection)/2;
            
		offsets.x = Experiment.X_offset;
		offsets.y =	Experiment.Y_offset;
		offsets.z = Experiment.Z_offset;
		if( !float.IsNaN(cameraRaycast.x) && !float.IsNaN(cameraRaycast.y) && !float.IsNaN(cameraRaycast.z) ){
			transform.position = smiInstance.transform.position + cameraRaycast * 10;
		}
		
	}
	
    // Update is called once per frame
    void Update () {
		
        if (Stimulus.GazeContingent)
        { UpdateWithGazePosition(); }
	
		// Make the stimulus facing user
		if( smiInstance != null ){
            transform.LookAt(smiInstance.transform);
			transform.rotation *= Quaternion.Euler(0,180,0);
        }
    }
}
