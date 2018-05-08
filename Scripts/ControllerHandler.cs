﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

/// <summary>
/// This class inherits from "SteamVR_TrackedController" script.
/// Button functions will be called once.
/// </summary>
/// <example>
/// <code>
/// public override void OnMenuClicked(ClickedEventArgs e)
/// {
///     base.OnMenuClicked(e);
///     // Extra things you wish to do.
/// }
/// </code>
/// </example>
public class ControllerHandler : SteamVR_TrackedController {
		
	private GameObject gameManager;
	private ResponseGetter response_script;
	private Vector2 touchPos;
	private Vector2 unTouchPos;

    /// <summary>
    /// This function tells GameManager to generate stimulus.
    /// </summary>
    public override void OnTriggerClicked(ClickedEventArgs e)
	{
		base.OnTriggerClicked(e);
		
		gameManager.GetComponent<GameManager>().AcceptSignal();	
	}

    /// <summary>
    /// This function tells ResponseGetter that user clicks pad.
    /// </summary>
    public override void OnPadClicked(ClickedEventArgs e)
	{
		base.OnPadClicked(e);
		
		response_script.SetTouchResponse(true);
	}

    /// <summary>
    /// This function ResponseGetter that user unclicks pad.
    /// </summary>
    public override void OnPadUnclicked(ClickedEventArgs e)
	{
		base.OnPadUnclicked(e);
		
		response_script.SetTouchResponse(false);
	}
	
	/// <summary>
	/// This function records the position where user touches pad.
	/// </summary>
	public override void OnPadTouched(ClickedEventArgs e)
	{
		base.OnPadTouched(e);
		
		touchPos.x = e.padX;
		touchPos.y = e.padY;
	}
	
	/// <summary>
	/// This function records the position where user leaves pad.
	/// </summary>
	public override void OnPadUntouched(ClickedEventArgs e)
	{
		base.OnPadUntouched(e);
		
		unTouchPos.x = controllerState.rAxis0.x;
		unTouchPos.y = controllerState.rAxis0.y;
        DeterminePadDirection();
	}

    // Use this for initialization
    protected override void Start () {
		base.Start();
		
		// Find the game manager and ResponseGetter object
		gameManager = GameObject.Find("Game Manager");	
		GameObject response_obj = GameObject.Find("ResponseModule");
		response_script = response_obj.GetComponent<ResponseGetter>();
	}
	
	// Update is called once per frame
	protected override void Update()
	{
		base.Update();	
	}
	
	
	/// <summary>
	/// This function determines the direction where user swipes the pad.
	/// </summary>
	public void DeterminePadDirection(){
		Vector2 direction = unTouchPos - touchPos;
		if( direction.y > 0.3 ){
			Debug.Log("Up");
			response_script.SetSwipeResponse(0);
		} else if( direction.y < -0.3 ){
			Debug.Log("Down");
			response_script.SetSwipeResponse(1);
		}
		if( direction.x < -0.3 ){
			Debug.Log("Left");
			response_script.SetSwipeResponse(2);
		} else if( direction.x > 0.3 ){
			Debug.Log("Right");
			response_script.SetSwipeResponse(3);
		}
	}
	
}
