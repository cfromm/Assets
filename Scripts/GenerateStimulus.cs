using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    }
	
	/// <summary>
	/// This function destroys the current stimulus instantly
	/// </summary>
	private void DestroyStim(){
		if( thisStim != null ){
			Destroy( thisStim );
		}		
	}

    
    public GameObject StimulusGenerator(float col_low, float col_high)
    {
        string[] stims;
        GameObject thisStim = (GameObject)Instantiate(Resources.Load("TextStimulus"));
		thisStim.gameObject.tag = "Stimulus";
        thisStim.SetActive(true);
        TextMesh stimComponent = null;
        stims = Stimulus.Letter.Split(',');
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
		
		// Destroy(obj, float) has the same effect
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
	/// Tells "ResponseGetter.cs" to start fetching user response
	/// </summary>
    public void StimulusEvent()
    {
        thisStim = StimulusGenerator(gameManager.current_color[0], gameManager.current_color[1]);
		
		// after generating the stimulus, start waiting for user response
		GameObject response_obj = GameObject.Find("ResponseModule");
		string requested = thisStim.GetComponent<TextMesh>().text;
		response_obj.GetComponent<ResponseGetter>().GetResponseEvent(requested);
    }

}
	