using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;

public class CalAssesment : MonoBehaviour
{
    [Tooltip("This button allows the experimenter to cycle through the assesment points")]
    [SerializeField]
    KeyCode ChangePoint = KeyCode.Space;

    [Tooltip("This button saves the angular error between the gaze and the true location")]
    [SerializeField]
    KeyCode RecordValue = KeyCode.R;
   
    public enum Eyetracker{ SMI, Pupil };
    [Tooltip("Select the eyetracker you are using")]
    [SerializeField]
    public Eyetracker eyetracker = Eyetracker.SMI;




    SMI.SMIEyeTrackingUnity smiInstance = null;
    public Vector2 planeGazeHit;
    public List<GameObject> Points;
    public GameObject currentPoint;
    public int PointIndex= 0;
    public Vector2 planeLocation;
    public Vector2 planeDisplacement;
    public List<float> AngularErrors;




    void FindCurrentPoint()
    {
        PointIndex += 1;
        currentPoint.SetActive(false);
        currentPoint = Points[PointIndex];
        currentPoint.SetActive(true);
    }

    void GetGazeError()
    {
        if (PointIndex < 9)
        {
            if (eyetracker == Eyetracker.SMI)
            {
                RaycastHit hitInformation;
                smiInstance.smi_GetRaycastHitFromGaze(out hitInformation);
                planeGazeHit = smiInstance.transform.rotation * hitInformation.point ;
                Debug.Log("gaze intersection is: " +planeGazeHit);
            }
            if (eyetracker == Eyetracker.Pupil)
            {
                Debug.Log("PupilLabs eyetracker not yet supported");
                planeGazeHit = Vector2.zero;
            }


            Vector2 planeLocation = currentPoint.transform.position;

            planeDisplacement = planeLocation - planeGazeHit;
            AngularErrors.Add(planeDisplacement.magnitude);
            Debug.Log("Gaze Error for point " + PointIndex + " is: " + planeDisplacement.magnitude);
        }
        else
        { Debug.Log("Calibration Assesment complete."); }
        }



        // Use this for initialization
        void Start()
        {
        if (eyetracker == Eyetracker.SMI)
        {
            smiInstance = SMI.SMIEyeTrackingUnity.Instance;
        }
        if (eyetracker == Eyetracker.Pupil)
        {
            Debug.Log("PupilLabs eyetracker not yet supported");
        }
        foreach (Transform child in transform)
         { 
             Points.Add(child.gameObject);
         }
        currentPoint = Points[PointIndex];
        currentPoint.SetActive(true);


        }

        // Update is called once per frame
        void Update()
        {
        if (Input.GetKeyDown(ChangePoint))
            { FindCurrentPoint(); }
        if (Input.GetKeyDown(RecordValue) && PointIndex < 9)
            { GetGazeError(); }

        }
        
    }
