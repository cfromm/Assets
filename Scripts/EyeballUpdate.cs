using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeballUpdate : MonoBehaviour {
    SMI.SMIEyeTrackingUnity smiInstance = null;

    void Start () {

            smiInstance = SMI.SMIEyeTrackingUnity.Instance;

        if (tag == "Left")
        {
            transform.position = smiInstance.transform.position + smiInstance.transform.rotation * (smiInstance.smi_GetLeftGazeBase());
        }
        if (tag == "Right")
        {
            transform.position = smiInstance.transform.position + smiInstance.transform.rotation * (smiInstance.smi_GetRightGazeBase());
        }

    }

   

    // Update is called once per frame
    void Update () {
        if (tag == "Left")
        {
            transform.position = smiInstance.transform.position+smiInstance.transform.rotation*( smiInstance.smi_GetLeftGazeBase());
        }
        if (tag == "Right")
        {
            transform.position =smiInstance.transform.position + smiInstance.transform.rotation * ( smiInstance.smi_GetRightGazeBase());
        }

    }
}

