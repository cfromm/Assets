using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;    // For lambda-expression

public class ResponseGetter : MonoBehaviour {

    public GameManager gameManager;
	public bool waitingforResponse;
	private string true_val;   // Stimulus direction or text
	private string user_resp;
	private bool isPoint;
	private bool isTouch;
	private bool[] user_direction;    // up, down, left, right		
	
    private void Awake()
    {       
		user_direction = new bool[4];
		ResetMonitorParam();
    }
	
	/// <summary>
	/// Reset all monitoring parameter back to false.
	/// </summary>
	public void ResetMonitorParam()
	{
		waitingforResponse = false;
		isPoint = false;
		isTouch = false;
		System.Array.Clear(user_direction, 0, user_direction.Length);
	}
	
	/// <summary>
	/// Set up the desired string and start receiving user response/input.
	/// </summary>
	/// <param name="request">Stimulus direction or text.</param>
    public void SetResponseEvent( string request = null )
    {
		// reset to prevent instant judge
		ResetMonitorParam();
		true_val = request;	
		waitingforResponse = true;
    }
	
	/// <summary>
	/// Set up isPoint for InputJudge.
	/// </summary>
	public void GetPointResponse( bool isTrue )
	{
		isPoint = isTrue;
	}
	
	/// <summary>
	/// Set up isTouch for InputJudge.
	/// </summary>
	public void GetTouchResponse( bool isTrue )
	{
		isTouch = isTrue;
	}
	
	/// <summary>
	/// Set up the user's direction for InputJudge.
	/// </summary>
	/// <param name="direction">Used to indicate direction.</param>
	public void GetSwipeResponse( int direction )
	{
		// clear last record
		System.Array.Clear(user_direction, 0, user_direction.Length);
		user_direction[direction] = true;
	}

	/// <summary>
	/// See if the user's response matches the stimulus
	/// and tell the game manager the result
	/// </summary>
    public void InputJudge()
    {   
		// Debug.Log("true vale: " + true_val);
		
		if( Experiment.InputMethod == "t" && Stimulus.Type == "t" )
		{			
			// Controller counts as input as well, but its string's length is 0
			if ( Input.anyKeyDown && Input.inputString.Length > 0 )
			{
				// Convert the input to uppercase since the stimulus is in uppercases
				user_resp = Input.inputString.ToUpper();
				Debug.Log( "User entered: " + user_resp );
				
				bool isTrue = true_val.Equals(user_resp);			
				gameManager.UserResponse(isTrue, gameManager.fixation_break);
				
				// Finish judging, no longer waiting for response
				ResetMonitorParam();
			}
		} else if( Experiment.InputMethod == "pc" )
		{			
			// When user clicks the pad
			if( isTouch == true )
			{
				// If user points at stimulus, pass true to game manager.
				gameManager.UserResponse(isTouch, isPoint);
				ResetMonitorParam();
			}
		} else if( Experiment.InputMethod == "ps" )
		{
			// When user points at stimulus or swipes the pad
			if( isPoint == true || user_direction.Any(x => x) )
			{
				if( ( true_val.Equals("Up") && user_direction[0] ) ||
					( true_val.Equals("Down") && user_direction[1] ) ||
					( true_val.Equals("Left") && user_direction[2] ) ||
					( true_val.Equals("Right") && user_direction[3] ) )
				{
					gameManager.UserResponse(true, isPoint);
				} else
				{ 
					gameManager.UserResponse(false, isPoint); 
				}
				
				ResetMonitorParam();
			}
		} else if( Experiment.InputMethod == "s" )
		{
			// When user swipes the pad
			if( user_direction.Any(x => x) )
			{
				if( ( true_val.Equals("Up") && user_direction[0] ) ||
					( true_val.Equals("Down") && user_direction[1] ) ||
					( true_val.Equals("Left") && user_direction[2] ) ||
					( true_val.Equals("Right") && user_direction[3] ) )
				{
					gameManager.UserResponse(true, gameManager.fixation_break);
				} else
				{ 
					gameManager.UserResponse(false, gameManager.fixation_break); 
				}
				
				ResetMonitorParam();
			}
		}

    }
	
	// Update is called once per frame
	void Update () 
	{		
		if( waitingforResponse && !gameManager.fixation_break )
		{
			InputJudge();
		}
        if(gameManager.fixation_break)
        {
            ResetMonitorParam();
            EventManager.TriggerEvent("DestroyStim");
            gameManager.generate_state = true;
            gameManager.stimulus_present = false;
            gameManager.fixation_break = false;


           // Debug.Log("Trial not logged due to fixation loss");
        }
    }
	}
	

