using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// Test comment for versioning 

/// <summary>
/// This class generates and draws a stimulus
/// </summary>
public class GenerateStimulus : MonoBehaviour {

    public Vector3 stim_position;
    public Vector3 fix_position;
    public GameObject thisStim = null;
    public GameObject fixation_dot;
    public string[] stims;
    public GameManager gameManager;
    private UnityAction spawnStim;
    private int direction;
    public AudioClip onset_sound;
    public AudioClip fixation_loss_sound;
	
	private float var_low;
	public float var_high;
    private IEnumerator wait;

    private void Awake()
    {
        spawnStim = new UnityAction(StimulusEvent);
    }

    public void OnEnable()
    {
        EventManager.StartListening("spawnStim", spawnStim);
		EventManager.StartListening("DestroyStim", DestroyStim);
        //EventManager.StartListening("DestroyFixation", DestroyFixation);
    }
    public void OnDisable()
    {
        EventManager.StopListening("spawnStim", spawnStim);
		EventManager.StopListening("DestroyStim", DestroyStim);
        //EventManager.StopListening("DestroyFixation", DestroyFixation);
    }


    private IEnumerator RemoveAfterSeconds(float seconds, GameObject stimulus)
    {		
        yield return new WaitForSecondsRealtime(seconds);
        //Debug.Log("Waiting to destroy");
        Destroy( stimulus );
        //Debug.Log("Stimulus has been destroyed");
		gameManager.stimulus_present = false;
    }

    private IEnumerator waitITI(float seconds_iti, float seconds_stim, GameObject fixation, GameManager gameManager)
    {
       
        Debug.Log("waiting for intertrial interval");
        gameManager.waitingITI = true;
        yield return new WaitForSecondsRealtime(seconds_stim);
        fixation.SetActive(false);
        yield return new WaitForSecondsRealtime(seconds_iti- seconds_stim);
        gameManager.waitingITI = false;
        fixation.SetActive(true);


    }


	/// <summary>
	/// This function destroys the current stimulus instantly
	/// </summary>
	private void DestroyStim()
	{
		if( thisStim != null )
		{
			Destroy( thisStim );
			gameManager.stimulus_present = false;
		}		
	}
    private void DestroyFixation()
    {
        if (fixation_dot != null)
        { Destroy(fixation_dot); }
    }

    private Vector3 validFixationPick()
    {
        if (Experiment.Fixation_Roving) // picks a new fixation location each trial
        {
            Vector3 fixation_pick = new Vector3(Random.Range(-Experiment.Roving_outer_vertical, Experiment.Roving_outer_vertical), Random.Range(-Experiment.Roving_outer_horizontal, Experiment.Roving_outer_horizontal), 0);
            while (gameManager.fixation_location.magnitude - Experiment.Roving_step < fixation_pick.magnitude && fixation_pick.magnitude <  gameManager.fixation_location.magnitude + Experiment.Roving_step )
            {
                //need to fix this to include absolute value and length instead of components.
                fixation_pick = new Vector3(Random.Range(-Experiment.Roving_outer_vertical, Experiment.Roving_outer_vertical), Random.Range(-Experiment.Roving_outer_horizontal, Experiment.Roving_outer_horizontal), 0);
            }
            Debug.Log(fixation_pick);
            return fixation_pick;
            
        }
        
        else
        {
            return new Vector3(0, 0, 0);
        }
    }

	/// <summary>
	/// This function sets the color range of the stimulus
	/// </summary>
	public void GetStimRange()
	{
        if (gameManager.current_staircase == 1)
        {
            gameManager.current_level = gameManager.level_1;
        }
        if (gameManager.current_staircase == 2)
        {
            gameManager.current_level = gameManager.level_2;
        }
        if (gameManager.current_staircase == 3)
        {
            gameManager.current_level = gameManager.level_3;
        }


        if (Stimulus.Type == "t")
        {
            float level_size = Stimulus.Max_Color / Experiment.Num_Levels;
            var_low = Stimulus.Max_Color - (gameManager.current_level) * level_size;
            var_high = Stimulus.Max_Color - (gameManager.current_level + 1) * level_size;
        }
        if (Stimulus.Type == "d")
        {

            string[] angles = Stimulus.Angle.Split(',');
            var_high = float.Parse(angles[gameManager.current_level]);
            gameManager.current_angle = var_high.ToString();
        }
        gameManager.fixation_location = validFixationPick();
                
        
	}
    
	/// <summary>
	/// Generate the stimulus based on its type 
	/// </summary>
    public GameObject StimulusGenerator()
    {
		GetStimRange();
        string[] stims = null;

        if (Stimulus.Type == "t")  //if new stimulus type is desired add to this section
        {
            GameObject thisStim = (GameObject)Instantiate(Resources.Load("TextStimulus"));
            thisStim.gameObject.tag = "Stimulus";
            thisStim.SetActive(true);
            TextMesh stimComponent = null;
            stims = Stimulus.Letter.Split(',');
            stimComponent = thisStim.GetComponent<TextMesh>();
            stimComponent.text = stims[Random.Range(0, stims.Length)];
            stimComponent.color = new Color (0f, 0f, 0f, Random.Range(var_low, var_high));
            //StartCoroutine(RemoveAfterSeconds(Stimulus.Duration, thisStim));
            return thisStim;
		}
        if (Stimulus.Type == "d")
        {
            stims = Stimulus.Directions.Split(',');
            GameObject thisStim = (GameObject)Instantiate(Resources.Load("DotStimulus"));
            thisStim.gameObject.tag = "Stimulus";
            thisStim.GetComponent<DotStimScript>().max_angle = var_high; 

            //StartCoroutine( RemoveAfterSeconds(Stimulus.Duration, thisStim) );
            return thisStim;

        }

    

        Invoke("DestroyStim", Stimulus.Duration);
        
        return thisStim;
    }


    public GameObject DrawFixation()
    {
        //fix_position = new Vector3(Experiment.X_Fixation, Experiment.Y_Fixation, Experiment.Z_Fixation);
        fixation_dot = (GameObject)Instantiate(Resources.Load("FixationDot"));
        fixation_dot.SetActive(false);
        //Invoke("DestroyFixation", Stimulus.Duration);
        return fixation_dot;
    }

	
	/// <summary>
	/// This function generates the stimulus
	/// Tells game manager the generated stimulus information
	/// Tells "ResponseGetter.cs" to start fetching user response
	/// </summary>
    public void StimulusEvent()
    {
        AudioSource audio = GetComponent<AudioSource>();
        thisStim = StimulusGenerator();

        //Invoke("DestroyStim", Stimulus.Duration);
        string requested = null;
        // after generating the stimulus, start waiting for user response	
        if (Stimulus.Type == "t")
        {
        requested = thisStim.GetComponent<TextMesh>().text;
        gameManager.current_text = requested;
        gameManager.current_color = thisStim.GetComponent<TextMesh>().color;
        }
        if (Stimulus.Type == "d")
        {
            direction = Random.Range(0, 2);
            thisStim.GetComponent<DotStimScript>().stim_direction = direction;
            if (direction == 0)
            {
                requested = "Right";
                gameManager.current_direction = "Right";
                Debug.Log("correct answer is: Right");
            }
            if (direction == 1)
            {
                requested = "Left";
                gameManager.current_direction = "Left";
                Debug.Log("correct answer is: Left");
            }

        }
        
        if (!gameManager.fixation_break)
        {
            Debug.Log("Gaze Error is: " + gameManager.angular_gaze_error);
            thisStim.SetActive(true);
            audio.clip = onset_sound;
            audio.Play();
            wait = waitITI(Stimulus.ITI, Stimulus.Duration, fixation_dot, gameManager);
            gameManager.waitingITI = false;
            StartCoroutine(wait);
            //gameManager.waitingITI = false;


        }
        else {
            //Invoke("DestroyFixation", 0.01f);
            audio.clip = fixation_loss_sound;
            audio.Play();
            Debug.Log("Fixation Break");
            //gameManager.fixation_break = true;
          
            return;

        }

		GameObject response_obj = GameObject.Find("ResponseModule");
		response_obj.GetComponent<ResponseGetter>().SetResponseEvent(requested);


    }

    public void Start()
    {
        fixation_dot = DrawFixation();
        fixation_dot.SetActive(true);
        
    }
}
	