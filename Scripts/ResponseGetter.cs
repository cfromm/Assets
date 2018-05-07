using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;    // For lambda-expression

/// <summary>
/// This class determines if the user response matches the stimulus.
/// </summary>
public class ResponseGetter : MonoBehaviour {

    public GameManager gameManager;
	public bool waitingforResponse;

    /// <summary>
    /// Stimulus direction or text.
    /// </summary>
	private string true_val;
	private string user_resp;
	private bool isPoint;
	private bool isTouch;

    /// <summary>
    /// Index 0 to 3: up, down, left, right.
    /// </summary>
	private bool[] user_direction;		

    // Use this for initialization
    public void Start()
    {       
		user_direction = new bool[4];
		ResetMonitorParam();
    }
	
	/// <summary>
	/// Reset all monitoring parameters back to false.
	/// </summary>
	public void ResetMonitorParam()
	{
		waitingforResponse = false;
		isPoint = false;
		isTouch = false;
		System.Array.Clear(user_direction, 0, user_direction.Length);
	}

    /// <summary>
    /// Set up the desired string and start waiting for user response.
    /// </summary>
    /// <param name="request">The stimulus direction or text.</param>
    public void SetResponseEvent( string request = null )
    {
		// reset to prevent instant judge
		ResetMonitorParam();
		true_val = request;	
		waitingforResponse = true;
    }

    /// <summary>
    /// This function tells if the user is pointing at the stimulus.
    /// </summary>
    /// <param name="isTrue">If the user is pointing at the stimulus.</param>
    public void SetPointResponse( bool isTrue )
	{
		isPoint = isTrue;
	}

    /// <summary>
    /// This function tells if the user clikced controller's pad.
    /// </summary>
    /// <param name="isTrue">If the user clikced controller's pad.</param>
    public void SetTouchResponse( bool isTrue )
	{
		isTouch = isTrue;
	}

    /// <summary>
    /// Set up the user's response direction.
    /// </summary>
    /// <param name="direction">Used to indicate direction.</param>
    public void SetSwipeResponse( int direction )
	{
		// clear last record
		System.Array.Clear(user_direction, 0, user_direction.Length);
		user_direction[direction] = true;
	}

	/// <summary>
	/// See if the user's response matches the stimulus and tell the game manager the result.
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
				gameManager.UserResponse(isTrue);
				
				// Finish judging, no longer waiting for response
				ResetMonitorParam();
			}
		} else if( Experiment.InputMethod == "pc" )
		{			
			// When user clicks the pad
			if( isTouch == true )
			{
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
					gameManager.UserResponse(true);
				} else
				{ 
					gameManager.UserResponse(false); 
				}
				
				ResetMonitorParam();
			}
		}

    }

    // Update is called once per frame
    public void Update () 
	{		
		if( waitingforResponse )
		{
			InputJudge();
		}
	}
	
}

