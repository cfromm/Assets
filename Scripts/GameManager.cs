﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;

public class GameManager : MonoBehaviour {
    private GameObject StimObject;
    private GenerateStimulus StimScript;
    public static GameManager instance = null;
    public int current_level = 0;
    public int running_consecutive_correct = 0;
	
	// true if the scene is ready to generate the next stimulus
    public bool generate_state;	
    public bool fixation;
    //private bool response_match; //whether the user response is correct
    public int trial_number = 0;
    public float[] current_color;
    public bool ExperimentComplete;


	SMI.SMIEyeTrackingUnity smiInstance = null;
	
    Vector2 binocularPor;
    Vector3 cameraRaycast;
    float ipd;
    Vector2 leftPor;
    Vector2 rightPor;
    Vector3 leftBasePoint;
    Vector3 rightBasePoint;
    Vector3 leftGazeDirection;
    Vector3 rightGazeDirection;
	
	GameObject GenerateStimulus;
	Vector3 stimulus_offset;
	Vector3 stimulus_fixation;
	char stimulus_letter;
	
	[Tooltip("Press to start/stop recording SMI eye tracker data.")]
    [SerializeField]
    KeyCode trigger1 = KeyCode.Space;
	FileStream streams;
	StringBuilder stringBuilder = new StringBuilder();
	String writeString;		
	Byte[] writebytes;
	bool startWrite = false;
	

    private void Awake()
    {
        if (instance == null)
        { instance = this; }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void OnApplicationQuit()
    {
        instance = null;
		streams.Close();
        Destroy(gameObject);	
    }


	/// <summary>	
	/// This function spawns stimulus if generate_state is true
	/// It should only be called by controller's trigger button
	/// </summary>
    public void AcceptSignal()
    {
		if( generate_state ){
            if ( current_color.Length != 2 )
            { current_color = LevelCounter(); }

			EventManager.TriggerEvent("spawnStim");
			generate_state = false;
		}		
    }
	
	/// <summary>
	/// This function is called when the user answers
	/// </summary>
	/// <param name="isTrue">Does the user response match the stimulus</param>
	public void UserResponse( bool isTrue ){

		if( isTrue ){
			running_consecutive_correct += 1;
			if( running_consecutive_correct == 3 ){
				current_level += 1;
				running_consecutive_correct = 0;
				current_color = LevelCounter();
			}
		} else{
			running_consecutive_correct = 0;
			if( current_level > 0 ){
				current_level -= 1;
				current_color = LevelCounter();
			}
		}
			
		// Destroy the stimulus and set generate_state back to true
		EventManager.TriggerEvent("DestroyStim");
		generate_state = true;
		
		TrialCounter();
	}

	/// <summary>
	/// Change the range of the color of the stimulus based on the current level
	/// Should be called only if the level changes
	/// </summary>
    public float[] LevelCounter()
    {
        float[] color_range = new float[2];

        float level_size = Stimulus.Max_Color / Experiment.Num_Levels;
        color_range[1] = Stimulus.Max_Color - (current_level) * level_size;
        color_range[0] = Stimulus.Max_Color - (current_level + 1) * level_size;
        //Debug.Log("Current color range is " + "(" + color_range[0] + " , " + color_range[1] + ")");
        return color_range;
    }
	
	/// <summary>
	/// This function sees if the total number of trials is matched
	/// If so, close the application???
	/// </summary>
	/// <remark>
	/// trial: a stimulus generated and a user response counts as one trial
	/// </remark>
    public void TrialCounter()
    {
		trial_number += 1;
        if (trial_number < Experiment.Trials)
        {
            ExperimentComplete = false;					
        }
        else { ExperimentComplete = true; }
    }


    public void Start()
    {
        StimObject = GameObject.Find("StimulusObject");
        StimScript = (GenerateStimulus)StimObject.GetComponent<GenerateStimulus>();

		generate_state = true;
		smiInstance = SMI.SMIEyeTrackingUnity.Instance;

		// create a folder based on "SaveLocation" from Json file and today's date
		String outputDir = Path.Combine(Experiment.SaveLocation, DateTime.Now.ToString("dd-MM-yyyy") );
		Directory.CreateDirectory( outputDir );	

		// create a file inside the folder based on the current time
		String outFileName = Path.Combine(outputDir, DateTime.Now.ToString("HH-mm") + ".txt");
		streams = new FileStream(outFileName, FileMode.Create, FileAccess.Write);
		WriteHeader();
    }

    /// <summary>
    /// Draw 3d gaze ray for binocular, left, and right eye in the editor. 
    /// </summary>
    void DrawRay()
    {
        // Note that you don't see this in the game scene. They are shown in the scene tab
        Debug.DrawRay(smiInstance.transform.position, cameraRaycast * 1000f, Color.blue);
        Debug.DrawRay(leftBasePoint, leftGazeDirection * 1000f, Color.red);
        Debug.DrawRay(rightBasePoint, rightGazeDirection * 1000f, Color.red);
    }

    /// <summary>
    /// Append the header information to the output file.
    /// </summary>
    void WriteHeader()
    {
        stringBuilder.Length = 0;
        stringBuilder.Append(DateTime.Now.ToString() + "\t" +
            "The file contains the information of the headset's position and rotation, " +
            ",the eye tracker's information, and stimulus's information " + Environment.NewLine +
            "The coordinate system is in Unity's world coordinate." + Environment.NewLine
            );
        stringBuilder.Append("--------------------------------------------" + Environment.NewLine
            );
        writeString = stringBuilder.ToString();

        writebytes = Encoding.ASCII.GetBytes(writeString);
        streams.Write(writebytes, 0, writebytes.Length);
    }

    /// <summary>
    /// Write information of the headset, eye tracker and stimulus in world coordinate.
    /// </summary>
    /// <remarks>
    /// Each time the method will generate:
    /// Time Stamp: milliseconds	User's Position: (x, y, z)	User's Rotation: (x, y, z)
    /// cameraRaycast: (-0.0768, -0.3463, 0.935)	binocularPor: (1073.616, 614.5098)	 ipd: 0.0678889	leftPor: (1097.505, 605.5203)	rightPor: (1049.728, 623.4992)	
    /// leftBasePoint: (0.7451017, 1.118986, 2.374621)	rightBasePoint: (0.8137214, 1.124029, 2.380525)	
    /// leftGazeDirection: (-0.02443469, -0.3211047, 0.946727)	rightGazeDirection: (-0.1286347, -0.3623178, 0.9231351)
    /// Number of stimulus: 1	Type: t	Letter: E	Color: red	stimulus offset: (x, y, z)	stimulus fixation: (x, y, z)
	/// </remarks>
    void WriteFile()
    {
        stringBuilder.Length = 0;
        stringBuilder.Append(
            "Time Stamp: " + Time.time * 1000 + "\t" +
            "User's Position: " + smiInstance.transform.position + "\t" +
            "User's Rotation: " + smiInstance.transform.eulerAngles + Environment.NewLine
            );
        stringBuilder.Append(
            "cameraRaycast: " + cameraRaycast.ToString("G4") + "\t" +
            "binocularPor: " + binocularPor + "\t" +
            " ipd: " + ipd + "\t" + 
			"leftPor: " + leftPor + "\t" +
            "rightPor: " + rightPor + Environment.NewLine
            );   		
		stringBuilder.Append(
            "leftBasePoint: " + leftBasePoint + "\t" +
            "rightBasePoint: " + rightBasePoint + Environment.NewLine
            );
		stringBuilder.Append(
            "leftGazeDirection: " + leftGazeDirection + "\t" +
            "rightGazeDirection: " + rightGazeDirection + Environment.NewLine
            );
		
        stimulus_offset.x = Experiment.X_offset;
		stimulus_offset.y = Experiment.Y_offset;
		stimulus_offset.z = Experiment.Z_offset;
		stimulus_fixation.x = Experiment.X_Fixation;
		stimulus_fixation.y = Experiment.Y_Fixation;
		stimulus_fixation.z = Experiment.Z_Fixation;
		stringBuilder.Append(
			"Number of trial: " + trial_number + "\t" +
			"Type: " + Stimulus.Type + "\t" +
			"Letter: " + Stimulus.Letter + "\t" +
			"Max Color: " + Stimulus.Max_Color + "\t" +
            "stimulus offset: " + stimulus_offset + "\t" +
			"stimulus fixation: " + stimulus_fixation + Environment.NewLine
            );
			
        writeString = stringBuilder.ToString();
        writebytes = Encoding.ASCII.GetBytes(writeString);
        streams.Write(writebytes, 0, writebytes.Length);

    }


    public void Update()
    {	
        //fixation = true;
        //GetGazePosition();

        //the origin is the camera
        binocularPor = smiInstance.smi_GetBinocularPor();
        cameraRaycast = smiInstance.transform.rotation * smiInstance.smi_GetCameraRaycast();
        ipd = smiInstance.smi_GetIPD();
        leftPor = smiInstance.smi_GetLeftPor();
        rightPor = smiInstance.smi_GetRightPor();
        leftBasePoint = smiInstance.transform.position + smiInstance.transform.rotation * smiInstance.smi_GetLeftGazeBase();
        rightBasePoint = smiInstance.transform.position + smiInstance.transform.rotation * smiInstance.smi_GetRightGazeBase();
        leftGazeDirection = smiInstance.transform.rotation * smiInstance.smi_GetLeftGazeDirection();
        rightGazeDirection = smiInstance.transform.rotation * smiInstance.smi_GetRightGazeDirection();
		
		DrawRay();
		
		if( Input.GetKeyDown(trigger1) ){		
			startWrite = !startWrite;
			if( startWrite ){
				Debug.Log("Start writing");
			} else{
				Debug.Log("Stop writing");
			}
		} 
				
		if( startWrite ){
			WriteFile();
		}
		
    }
	

}
    
       

