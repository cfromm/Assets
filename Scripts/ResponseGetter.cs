using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResponseGetter : MonoBehaviour {

    public GameManager gameManager;
    private UnityAction GetKeyInput;
	public bool waitingforResponse;
	private string true_val;
	private string user_resp;
	
    private void Awake()
    {
        //GetKeyInput = new UnityAction(GetResponseEvent);
		waitingforResponse = false;
    }
	
    //<summary> 
    // Starts the event Manager listening_
    private void OnEnable()
    {
        //EventManager.StartListening("GetKeyResponse", GetKeyInput);
    }

    private void OnDisable()
    {
        //EventManager.StopListening("GetKeyResponse", GetKeyInput);
    }

	
	/// <summary>
	/// Set up the desired string and start receiving user response/input
	/// </summary>
    public void GetResponseEvent( string request )
    {
		true_val = request;
		waitingforResponse = true;       
    }

	/// <summary>
	/// See if the user's response matches the stimulus
	/// and tell the game manager the result
	/// </summary>
    public void InputJudge()
    {   
		// Debug.Log("true vale: " + true_val);
		
		// Controller counts as input as well, but its string's length is 0
        if ( Input.anyKeyDown && Input.inputString.Length > 0 )
        {
			// Convert the input to uppercase since the stimulus is in uppercases
			user_resp = Input.inputString.ToUpper();
			Debug.Log( "User entered: " + user_resp );
			
			bool isTrue = true_val.Equals(user_resp);			
			gameManager.GetComponent<GameManager>().UserResponse(isTrue);
						
			// Finish judging, no longer waiting for response
			waitingforResponse = false;
        }
        //will this constantly listen? when to initiate new input
		
    }
	
	// Update is called once per frame
	void Update () {
		
		if( waitingforResponse ){
			InputJudge();
		}
	}
	
}

