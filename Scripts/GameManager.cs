using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;

public class GameManager : MonoBehaviour {
    private GameObject StimObject;
    private GenerateStimulus StimScript;
    public bool SaveBool;
    public static GameManager instance = null;
    public int current_staircase;
    public int current_level;
    public int level_1 = Experiment.Stair1_Init;
    public int running_consecutive_correct_1 = 0;
    public int level_2 = Experiment.Stair2_Init;
    public int running_consecutive_correct_2 = 0;
    public int level_3 = Experiment.Stair3_Init;
    public int running_consecutive_correct_3 = 0;
    public bool generate_state;	// if the scene is ready to generate the next stimulus
    //public bool fixation;
    public float angular_gaze_error;
    //private bool response_match; //whether the user response is correct
    public int trial_number = 0;
	public bool trial_success = false;  //whether the user response is correct
    public bool fixation_break = false;
    public bool waitingITI = false;//whether the user breaks fixation during the trial
    public bool stimulus_present = false;
    public Color current_color;
	public string current_text;
    public string current_angle;
    public string current_direction;
    public bool ExperimentComplete = false;
	private float stimStartTime;
    public AudioClip success_sound;
    public AudioClip fail_sound;
    public AudioClip complete_sound;
    

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
	Vector3 stimulus_offset;
	Vector3 stimulus_fixation;
	
	[Tooltip("Press to start/stop recording SMI eye tracker data.")]
    [SerializeField]
    KeyCode trigger1 = KeyCode.Space;
	FileStream streams;
	FileStream trialStreams;
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

	/// <summary>	
	/// Close the file streams
	/// </summary>
    private void OnApplicationQuit()
    {
        instance = null;
        if (SaveBool)
        {
            streams.Close();
            trialStreams.Close();
        }
        Destroy(gameObject);	
    }


	/// <summary>	
	/// This function spawns stimulus if generate_state is true
	/// It should only be called by controller's trigger button
	/// </summary>
    public void AcceptSignal()
    {
		if( generate_state  && !fixation_break && !waitingITI){
			trial_number += 1;
            current_staircase = UnityEngine.Random.Range(1, 4);
			EventManager.TriggerEvent("spawnStim");
			generate_state = false;
			stimulus_present = true;
			stimStartTime = Time.time * 1000;
		}		
    }
	
	/// <summary>
	/// This function is called after the user answers
	/// </summary>
	/// <param name="isTrue">Does the user response match the stimulus</param>
	public void UserResponse( bool isTrue, bool brokeFixation)
    {
        AudioSource sounds = GetComponent<AudioSource>();
        trial_success = isTrue;
		if( trial_success && !brokeFixation)
		{
            sounds.clip = success_sound;
            sounds.Play();
            TrialCounter(1);
            Debug.Log("Correct!");
		} 
        if(!trial_success && !brokeFixation)
		{
            sounds.clip = fail_sound;
            sounds.Play();
            Debug.Log("Incorrect");
			TrialCounter(0);
		}
        if (brokeFixation)
        {
            EventManager.TriggerEvent("DestroyStim");
            generate_state = true;
            stimulus_present = false;
            Debug.Log("Trial not logged due to fixation loss"); }
	}
	
	/// <summary>
	/// This function is called after the user answers
	/// </summary>
	/// <param name="isTrue">Does the user response match the stimulus</param>
	/// <param name="isPoint">Does the user point at the stimulus</param>
	public void UserResponse( bool isTrue, bool isPoint, bool isFixated )
	{
		trial_success = isTrue && isPoint && isFixated;      
		if( trial_success )
		{
			TrialCounter(2);
		} else if( isTrue )
		{
			TrialCounter(1);
		} else
		{
			TrialCounter(0);
		}
	}	
	
	/// <summary>
	/// This function records the trial result
	/// </summary>
	/// <param name="score">How many score does the user get from this trial</param>
	/// <remark>
	/// trial: a stimulus generated and a user response counts as one trial
	/// Trial#	Start		End			Correct	Exp.type	Sti.type	Res.type	Text	Color							
	/// -----------------------------------------------------------------------
	/// 1		1241.806	4618.559	False	a			t			m			R		RGBA(0.000, 0.000, 0.000, 0.796)
	/// </remark>
    public void TrialCounter( int score )
    {	
		AudioSource sounds = GetComponent<AudioSource>();
        if (score > 0 && current_staircase == 1)
		{
			running_consecutive_correct_1 += score;
			if( running_consecutive_correct_1 >= 3 )
			{
				level_1 += 1;
                if(level_1 > Experiment.Num_Levels-1)
                { level_1 = Experiment.Num_Levels - 1; }
				running_consecutive_correct_1 -= 3;
			}

		} 
        if (score <= 0 && current_staircase== 1)
		{
			running_consecutive_correct_1 = 0;
			if( level_1 > 0 )
			{
				level_1 -= 1;
			}

		}

        if (score > 0 && current_staircase == 2)
        {
            running_consecutive_correct_2 += score;
            if (running_consecutive_correct_2 >= 3)
            {
                level_2 += 1;
                if (level_2 > Experiment.Num_Levels - 1)
                { level_2 = Experiment.Num_Levels - 1; }
                running_consecutive_correct_2 -= 3;

            }
        }
        if (score <= 0 && current_staircase == 2)
        {
            running_consecutive_correct_2 = 0;
            if (level_2 > 0)
            {
                level_2 -= 1;
            }

        }

        if (score > 0 && current_staircase == 3)
            {
            running_consecutive_correct_3 += score;
            if (running_consecutive_correct_3 >= 3)
            {
                level_3 += 1;
                if (level_3 > Experiment.Num_Levels - 1)
                { level_3 = Experiment.Num_Levels - 1; }
                running_consecutive_correct_3 -= 3;
            }

        }
        if (score <= 0 && current_staircase == 3)
            {
            running_consecutive_correct_3 = 0;
            if (level_3 > 0)
            {
                level_3 -= 1;
            }

        }

        // Destroy the stimulus and set generate_state back to true
        EventManager.TriggerEvent("DestroyStim");
        //EventManager.TriggerEvent("DestroyFixation");

		generate_state = true;
		stimulus_present = false;
	
		// Record the result of this trial
		stringBuilder.Length = 0;
		if( Stimulus.Type.Equals("t") )
		{
			stringBuilder.Append
			(
				trial_number + "\t\t" + stimStartTime  + "\t" + Time.time*1000 + "\t" +
				trial_success + "\t" + "a\t\t\t" + Stimulus.Type + "\t\t\t" + 
				Experiment.InputMethod + "\t\t\t" + current_text + "\t\t" +
				current_color + Environment.NewLine
			);
		}
        if (Stimulus.Type.Equals("d"))
        {
            stringBuilder.Append
            (
                trial_number + "\t\t" + stimStartTime + "\t" + Time.time * 1000 + "\t" +
                current_angle + "\t"  + current_staircase + "\t\t" +
                trial_success + Environment.NewLine
            );
        }
        if (SaveBool)
        {
            writeString = stringBuilder.ToString();
            writebytes = Encoding.ASCII.GetBytes(writeString);
            trialStreams.Write(writebytes, 0, writebytes.Length);
        }
		
        if (trial_number >= Experiment.Trials)
        {
            ExperimentComplete = true;
            sounds.clip = complete_sound;
            sounds.Play();
			Debug.Log("Experiment Complete.");
            //StopEditorPlayback();
            Application.Quit();
        }
    }

    void StopEditorPlayback()
    {
      if (Application.isEditor)
        { 
       // UnityEditor.EditorApplication.isPlaying = false;
        }
    }

    public void Start()
    {
        StimObject = GameObject.Find("StimulusObject");
        StimScript = (GenerateStimulus)StimObject.GetComponent<GenerateStimulus>();
        SaveBool = Experiment.SaveBool;
        current_staircase = UnityEngine.Random.Range(0, 4);

		generate_state = true;
		smiInstance = SMI.SMIEyeTrackingUnity.Instance;

        if (SaveBool)
        {
            // create a folder based on "SaveLocation" from Json file and today's date
            String outputDir = Path.Combine(Experiment.SaveLocation, string.Concat(DateTime.Now.ToString("MM-dd-yyyy"), Experiment.SubjectIntials));
            Directory.CreateDirectory(outputDir);

            // create a file inside the folder based on the current time
            String outFileName = Path.Combine(outputDir, DateTime.Now.ToString("yyyy-MM-dd-HH-mm") + "_Parameters.txt");
            streams = new FileStream(outFileName, FileMode.Create, FileAccess.Write);

            // create another file to record trial results
            String trialOutput = Path.Combine(outputDir, DateTime.Now.ToString("yyyy-MM-dd-HH-mm") + "_Results.txt");
            trialStreams = new FileStream(trialOutput, FileMode.Create, FileAccess.Write);

            WriteHeader();
        }
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
    /// Append the header information to the output file and trial output file.
    /// </summary>
    void WriteHeader()
    {
		// output file
        stringBuilder.Length = 0;
        stringBuilder.Append(DateTime.Now.ToString() + "\t" +
            "The file contains the information of the headset's position and rotation, " +
            ",the eye tracker's information, and stimulus's information " + Environment.NewLine +
            "The coordinate system is in Unity's world coordinate." + Environment.NewLine
            );
        stringBuilder.Append("-------------------------------------------------" +
			Environment.NewLine
            );
        writeString = stringBuilder.ToString();
        writebytes = Encoding.ASCII.GetBytes(writeString);
        streams.Write(writebytes, 0, writebytes.Length);
		
		// trial output file
		stringBuilder.Length = 0;
        if (Stimulus.Type.Equals("t")) {
            stringBuilder.Append(
                "Trial#\t" + "Start\t\t" + "End\t\t\t" + "Correct\t" +
                "Exp.type\t" + "Sti.type\t" + "Res.type\t" +
                "Text\t" + "Color\t\t\t\t\t\t\t" + Environment.NewLine
            );
            stringBuilder.Append(
                "-----------------------------------------------------------------------" +
                Environment.NewLine
            ); }
        if (Stimulus.Type == "d")
            {
                stringBuilder.Append(
                    "TrialNumber\t" +"StartTime\t" + "EndTime\t"+ "CoherenceAngle\t" + "StaircaseNumber\t" + "Response\t" + Environment.NewLine
                    );
            }
		else{
			stringBuilder.Append(
				"Other types are not yet supported."
			);
		}
		writeString = stringBuilder.ToString();
		writebytes = Encoding.ASCII.GetBytes(writeString);
		trialStreams.Write(writebytes, 0, writebytes.Length);
		
    }

    /// <summary>
    /// Write information of the headset, eye tracker and stimulus in world coordinate.
    /// </summary>
    /// <remarks>
    /// Each time the method will generate:
    /// Time Stamp: 14360.88	User's Position: (0.8, 1.2, 2.4)	User's Rotation: (352.8, 262.3, 359.4)
	/// cameraRaycast: (-0.919, -0.2204, -0.3269)	binocularPor: (904.5, 750.5)	 ipd: 0.06165263	leftPor: (944.4, 759.9)	rightPor: (864.7, 741.1)
	/// leftBasePoint: (0.8, 1.2, 2.4)	rightBasePoint: (0.8, 1.2, 2.4)
	/// leftGazeDirection: (-0.9, -0.2, -0.3)	rightGazeDirection: (-0.9, -0.2, -0.4)
	/// Trial number: 2	Stimulus present: True	Experiment type: a	Stimulus type: t	Response type: m	level: 0	Text: P	
	/// </remarks>
    void WriteFile()
    {
        stringBuilder.Length = 0;
        if (Experiment.WriteFrameData)
        {
            // User's info
            stringBuilder.Append(
                "Time Stamp: " + Time.time * 1000 + "\t" +
                "User's Position: " + smiInstance.transform.position + "\t" +
                "User's Rotation: " + smiInstance.transform.eulerAngles + Environment.NewLine
                );

            // SMI eye tracker's info
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
        }

        if (Experiment.WriteTrialData && Stimulus.Type == "d")
        {
            // Stimulus info
            stringBuilder.Append(
                "Trial number: " + trial_number + "\t" +
                "Coherence Angle: " + stimulus_present + "\t" +
                "Staircase Number" + current_staircase + "\t" +
                "Response: " + trial_success + "\t" +
                //"level: " + current_level + "\t" +
                Environment.NewLine
                );
        }
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
        //Debug.Log(Time.deltaTime);
		DrawRay();
		
		if( Input.GetKeyDown(trigger1) ){		
			startWrite = !startWrite;
			if( startWrite ){
				Debug.Log("Start writing");
			} else{
				Debug.Log("Stop writing");
			}
		} 
				
		if( startWrite && SaveBool && Experiment.WriteFrameData){
			WriteFile();
		}
        if( startWrite && SaveBool && Experiment.WriteTrialData)
        {
            //on response
        }
		
    }
	

}
    
       

