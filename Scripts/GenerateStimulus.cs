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
    public GameObject fixationCross;
    public string[] stims;
    public GameManager gameManager;
    private UnityAction spawnStim;
    private int direction;
	
	private float var_low;
	public float var_high;

    private void Awake()
    {
        spawnStim = new UnityAction(StimulusEvent);
    }

    public void OnEnable()
    {
        EventManager.StartListening("spawnStim", spawnStim);
		EventManager.StartListening("DestroyStim", DestroyStim);
    }
    public void OnDisable()
    {
        EventManager.StopListening("spawnStim", spawnStim);
		EventManager.StopListening("DestroyStim", DestroyStim);
    }


    private IEnumerator RemoveAfterSeconds(float seconds, GameObject stimulus)
    {		
        yield return new WaitForSeconds(seconds);
        Debug.Log("Waiting to destroy");
        Destroy( stimulus );
        Debug.Log("Stimulus has been destroyed");
		gameManager.stimulus_present = false;
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
        }
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
            return thisStim;
		}
        if (Stimulus.Type == "d")
        {
            stims = Stimulus.Directions.Split(',');
            GameObject thisStim = (GameObject)Instantiate(Resources.Load("DotStimulus"));
            thisStim.gameObject.tag = "Stimulus";
            thisStim.GetComponent<DotStimScript>().max_angle = var_high; 
            thisStim.SetActive(true);
            
            //TimedDestroy();
            return thisStim;

        }

    

        Invoke("DestroyStim", Stimulus.Duration);
        //StartCoroutine( RemoveAfterSeconds(Stimulus.Duration, thisStim) );
        return thisStim;
    }


    public void DrawFixation()
    {
        fix_position = new Vector3(Experiment.X_Fixation, Experiment.Y_Fixation, Experiment.Z_Fixation);
        GameObject fixationCross = (GameObject)Instantiate(Resources.Load("FixationCross"));
        fixationCross.transform.position = fix_position;
    }

	
	/// <summary>
	/// This function generates the stimulus
	/// Tells game manager the generated stimulus information
	/// Tells "ResponseGetter.cs" to start fetching user response
	/// </summary>
    public void StimulusEvent()
    {
        thisStim = StimulusGenerator();
        Invoke("DestroyStim", Stimulus.Duration);
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
            direction = thisStim.GetComponent<DotStimScript>().stim_direction;
            if (direction == 0)
            {
                requested = "Right";
            }
            if (direction == 1)
            {
                requested = "Left";
            }

        }

        AudioSource audio = GetComponent<AudioSource>();
        audio.Play();

		GameObject response_obj = GameObject.Find("ResponseModule");
		response_obj.GetComponent<ResponseGetter>().SetResponseEvent(requested);

        DrawFixation();
    }

}
	