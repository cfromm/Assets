using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixationPositionUpdate : MonoBehaviour {

    SMI.SMIEyeTrackingUnity smiInstance = null;
    Vector3 fixationRaycast;
    Quaternion offsets;
    public Vector3 sizeinUnityUnits;
    public Vector3 displacementVector;
    public Vector3 gazeVector;
    public float angularError;
    public GameManager gameManager = GameManager.instance;
    public float fixation_timer;
    public Renderer rend;
    public Quaternion fixation_location;


    // Use this for initialization
    void Start()
    {
        smiInstance = SMI.SMIEyeTrackingUnity.Instance;
        fixationRaycast = smiInstance.transform.forward;
        if (!float.IsNaN(fixationRaycast.x) && !float.IsNaN(fixationRaycast.y) && !float.IsNaN(fixationRaycast.z) && Stimulus.GazeContingent)
        {
            transform.position = smiInstance.transform.position + fixationRaycast * 10;
        }
        rend = GetComponent<Renderer>();

        
    }


    /// <summary>
    /// Get the SMI gaze position and add the desired offset
    /// To be called in update if gaze-contingent stimulus is desired
    /// </summary>
    void UpdateWithGazePosition()
    {
         fixation_location = Quaternion.Euler( gameManager.fixation_location); 
    

        //if (!float.IsNaN(fixationRaycast.x) && !float.IsNaN(fixationRaycast.y) && !float.IsNaN(fixationRaycast.z))
        {

            if (Experiment.Monocular == "r")
            {
                //fixationRaycast = smiInstance.smi_GetRightGazeBase()+ smiInstance.transform.rotation* fixation_location * smiInstance.transform.forward ;
                fixationRaycast = smiInstance.smi_GetRightGazeBase() + smiInstance.transform.rotation * fixation_location * Vector3.forward;
                transform.position = smiInstance.transform.position + smiInstance.smi_GetRightGazeBase() + fixation_location * smiInstance.transform.forward * Stimulus.StimDepth;
                Debug.DrawRay(smiInstance.transform.position + smiInstance.smi_GetRightGazeBase(), smiInstance.transform.forward * Stimulus.StimDepth, Color.yellow);
                gazeVector = (smiInstance.transform.position +(smiInstance.transform.rotation *  (smiInstance.smi_GetRightGazeBase() + smiInstance.smi_GetRightGazeDirection()) * Stimulus.StimDepth));
                Debug.DrawRay((smiInstance.smi_GetRightGazeBase() + smiInstance.transform.position), smiInstance.transform.rotation * smiInstance.smi_GetRightGazeDirection() * Stimulus.StimDepth, Color.magenta);
                Debug.DrawRay((smiInstance.smi_GetLeftGazeBase() + smiInstance.transform.position), smiInstance.transform.rotation * smiInstance.smi_GetLeftGazeDirection() * Stimulus.StimDepth, Color.cyan);
            }
            if (Experiment.Monocular == "l")
            {
                //fixationRaycast =  smiInstance.smi_GetLeftGazeBase()+ fixation_location* smiInstance.transform.forward ;
                fixationRaycast = smiInstance.smi_GetLeftGazeBase() + smiInstance.transform.rotation * fixation_location * Vector3.forward;
                transform.position = smiInstance.transform.position + smiInstance.smi_GetLeftGazeBase() + fixation_location * smiInstance.transform.forward * Stimulus.StimDepth;
                //Debug.Log(transform.position);
                Debug.DrawRay(smiInstance.transform.position + smiInstance.transform.rotation * (smiInstance.smi_GetLeftGazeBase()), smiInstance.transform.forward*Stimulus.StimDepth, Color.yellow);
                gazeVector = ( smiInstance.transform.position  + (smiInstance.transform.rotation *(smiInstance.smi_GetLeftGazeBase() +  smiInstance.smi_GetLeftGazeDirection()) * Stimulus.StimDepth));
                Debug.DrawRay(smiInstance.transform.position + smiInstance.transform.rotation * (smiInstance.smi_GetRightGazeBase()), smiInstance.transform.rotation * smiInstance.smi_GetRightGazeDirection() * Stimulus.StimDepth, Color.magenta);
                Debug.DrawRay(smiInstance.transform.position + smiInstance.transform.rotation * (smiInstance.smi_GetLeftGazeBase()), smiInstance.transform.rotation * smiInstance.smi_GetLeftGazeDirection() * Stimulus.StimDepth, Color.cyan);
            }
            if (Experiment.Monocular == "b")
            {
                //fixationRaycast = fixation_location * smiInstance.transform.forward;
                fixationRaycast =  smiInstance.transform.rotation * fixation_location * Vector3.forward;
                //Debug.DrawRay(smiInstance.transform.position, fixation_location * smiInstance.transform.forward * Stimulus.StimDepth, Color.yellow);
                transform.position = smiInstance.transform.position + fixationRaycast * Stimulus.StimDepth;
                gazeVector = smiInstance.transform.position + smiInstance.transform.rotation * smiInstance.smi_GetCameraRaycast() * Stimulus.StimDepth;
                Debug.DrawRay(smiInstance.transform.position + smiInstance.transform.rotation * (smiInstance.smi_GetRightGazeBase()), smiInstance.transform.rotation * smiInstance.smi_GetRightGazeDirection() * Stimulus.StimDepth, Color.red);
                Debug.DrawRay(smiInstance.transform.position + smiInstance.transform.rotation * (smiInstance.smi_GetLeftGazeBase()), smiInstance.transform.rotation * smiInstance.smi_GetLeftGazeDirection() * Stimulus.StimDepth, Color.red);
            }
        }
            Debug.DrawRay(smiInstance.transform.position, smiInstance.transform.rotation * smiInstance.smi_GetCameraRaycast() * Stimulus.StimDepth , Color.blue);
            //Debug.Log(smiInstance.smi_GetRightGazeBase());
            //transform.position = smiInstance.transform.position + fixationRaycast * Stimulus.StimDepth;
            
            displacementVector = gazeVector - transform.position;
            transform.localScale = new Vector3(2 * Mathf.Tan((Experiment.FixationRad * Mathf.PI) / 180) * Stimulus.StimDepth, 0, 2 * Mathf.Tan(Experiment.FixationRad * Mathf.PI / 180) * Stimulus.StimDepth);
        

        Vector3 head = UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.Head)*1000;
        Vector3 eye = UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.LeftEye)*1000;
        Vector3 smiLeft = smiInstance.smi_GetLeftGazeBase()* 1000;
        Vector3 smiRight = smiInstance.smi_GetRightGazeBase() * 1000;
        //Debug.Log("Left eye mm" + smiLeft);
        //Debug.Log("Right eye mm" + smiRight);
        //Debug.Log("Difference is: " + (UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.Head) - UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.LeftEye)));
    
        //Debug.Log(UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.CenterEye));
    }

    private void Awake()
    {
        gameManager = GameManager.instance; 
    }
    void Update()
    {
       // angularError = smiInstance.smi_GetCameraRaycast() * Stimulus.StimDepth - transform.position;
        //set up size conversions to degrees here
        if (Stimulus.GazeContingent)
        { UpdateWithGazePosition();
            angularError = 2*Mathf.Sin((displacementVector.magnitude /2)/Stimulus.StimDepth)*180/Mathf.PI;
            gameManager.angular_gaze_error = angularError;
            //Debug.Log("Angular Error is: " + angularError);
            if (angularError > Experiment.Fixation_zone && fixation_timer < Stimulus.FixationDuration)
            {   gameManager.fixation_break = true;
                rend.material.color = Color.black;
                fixation_timer = 0;
            }
            if( angularError < Experiment.Fixation_zone  && fixation_timer < Stimulus.FixationDuration)
            {
                fixation_timer += 1/90f;
                rend.material.color = Color.yellow;
                gameManager.fixation_break = false;

            }
            if (fixation_timer > Stimulus.FixationDuration)
            {
                gameManager.fixation_break = false;
                rend.material.color = Color.black;
                gameManager.GetComponent<GameManager>().AcceptSignal();
                fixation_timer = 0;
            }
            if (gameManager.stimulus_present) 
            { fixation_timer = 0; }

            
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

