using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

/// <summary>
/// This class inherits from "SteamVR_TrackedController" script.
/// </summary>
/// <example>
/// <code>
/// public override void OnMenuClicked(ClickedEventArgs e)
/// {
///     base.OnMenuClicked(e);
/// }
/// </code>
/// </example>
public class ControllerHandler : SteamVR_TrackedController {
	
	private GameObject gameManager;
	private Vector2 touchPos;
	private Vector2 unTouchPos;
	
	/// <summary>
	/// This function calls "AcceptSignal" method in GameManager's script
	/// </summary>
	/// <remarks>
	/// This function will be called once trigger has been pressed
	/// </remarks>
	public override void OnTriggerClicked(ClickedEventArgs e)
	{
		base.OnTriggerClicked(e);
		
		gameManager.GetComponent<GameManager>().AcceptSignal();	
	}
	
	/// <summary>
	/// This function records the position where user touches pad
	/// </summary>
	public override void OnPadTouched(ClickedEventArgs e)
	{
		base.OnPadTouched(e);
		
		touchPos.x = e.padX;
		touchPos.y = e.padY;
	}
	
	/// <summary>
	/// This function records the position where user leaves pad
	/// </summary>
	public override void OnPadUntouched(ClickedEventArgs e)
	{
		base.OnPadUntouched(e);
		
		unTouchPos.x = controllerState.rAxis0.x;
		unTouchPos.y = controllerState.rAxis0.y;
		DeterminPadDirection();
	}

    // Use this for initialization
    protected override void Start () {
		base.Start();
		
		// Find the game manager object
		gameManager = GameObject.Find("Game Manager");			
	}
	
	// Update is called once per frame
	protected override void Update()
	{
		base.Update();	
	}
	
	
	/// <summary>
	/// This function determines the direction where user swipes the pad
	/// </summary>
	public void DeterminPadDirection(){
		Vector2 direction = unTouchPos - touchPos;
		if( direction.y > 0.3 ){
			Debug.Log("Up");
		} else if( direction.y < -0.3 ){
			Debug.Log("Down");
		}
		if( direction.x > 0.3 ){
			Debug.Log("Right");
		} else if( direction.x < -0.3 ){
			Debug.Log("Left");
		}
	}
	
}
