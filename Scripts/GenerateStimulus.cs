﻿using System.Collections;
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
	
	private float col_low;
	private float col_high;

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


    private IEnumerator RemoveAfterSeconds(float seconds)
    {		
        yield return new WaitForSeconds(seconds);
        Destroy( thisStim );
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
	public void GetColorRange()
	{
        float level_size = Stimulus.Max_Color / Experiment.Num_Levels;
        col_low = Stimulus.Max_Color - (gameManager.current_level) * level_size;
        col_high = Stimulus.Max_Color - (gameManager.current_level + 1) * level_size;
	}
    
	/// <summary>
	/// Generate the stimulus based on its type 
	/// </summary>
    public GameObject StimulusGenerator()
    {
        GameObject thisStim = (GameObject)Instantiate(Resources.Load("TextStimulus"));
		thisStim.gameObject.tag = "Stimulus";
        thisStim.SetActive(true);
        TextMesh stimComponent = null;	
		GetColorRange();
		
        string[] stims = Stimulus.Letter.Split(',');
        if (Stimulus.Type == "t")  //if new stimulus type is desired add to this section
        {
            stimComponent = thisStim.GetComponent<TextMesh>();
            stimComponent.text = stims[Random.Range(0, stims.Length)];
            stimComponent.color = new Color (0f, 0f, 0f, Random.Range(col_low, col_high));			
		}
        ////Pending input method that can be performed inside the helmet
        //if (Experiment.InputMethod == "m") 
        //{ for(int i = 0; i < stims.Length; i++)
        //    {
        //        GameObject ChoiceObj = thisStim;
        //        TextMesh choiceComponent = ChoiceObj.GetComponent<TextMesh>();
        //        choiceComponent.text = stims[i];
        //        choiceComponent.color = Color.black;
        //        ChoiceObj.AddComponent<BoxCollider>(); 
        //        ChoiceObj.SetActive(true);
        //    }
       // }

        else
        {
            print("Non-text stimulus not yet supported");
        }
		
		StartCoroutine( RemoveAfterSeconds(Stimulus.Duration) );
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
		
		// after generating the stimulus, start waiting for user response	
		string requested = thisStim.GetComponent<TextMesh>().text;

        AudioSource audio = GetComponent<AudioSource>();
        audio.Play();
        gameManager.current_text = requested;
		gameManager.current_color = thisStim.GetComponent<TextMesh>().color;
		GameObject response_obj = GameObject.Find("ResponseModule");
		response_obj.GetComponent<ResponseGetter>().SetResponseEvent(requested);
    }

}
	