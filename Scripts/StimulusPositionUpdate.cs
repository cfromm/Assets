using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StimulusPositionUpdate : MonoBehaviour {
    SMI.SMIEyeTrackingUnity smiInstance = null;
    Vector3 cameraRaycast;
	Quaternion offsets;
    public Vector3 sizeinUnityUnits;
	
    // Use this for initialization
    void Start() {
        smiInstance = SMI.SMIEyeTrackingUnity.Instance;
		offsets = Quaternion.Euler(Experiment.X_offset, Experiment.Y_offset, 0);
		
		cameraRaycast =  smiInstance.transform.rotation * offsets * smiInstance.smi_GetCameraRaycast();
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
        cameraRaycast = smiInstance.transform.rotation * offsets * smiInstance.smi_GetCameraRaycast();
        
		if( !float.IsNaN(cameraRaycast.x) && !float.IsNaN(cameraRaycast.y) && !float.IsNaN(cameraRaycast.z) ){
            transform.localPosition = smiInstance.transform.position + cameraRaycast * Stimulus.StimDepth; //scales magnitude of position by desired value
           // Debug.Log("Stimulus is " + 2*Mathf.Sin(Stimulus.ApertureRad * Mathf.PI / 180f)*Stimulus.StimDepth + " meters big, viewed at " +Stimulus.StimDepth);
            transform.localScale = new Vector3(2*Mathf.Sin((Stimulus.ApertureRad*Mathf.PI)/180) * Stimulus.StimDepth, 0, 2*Mathf.Sin(Stimulus.ApertureRad * Mathf.PI / 180) * Stimulus.StimDepth);
            
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
