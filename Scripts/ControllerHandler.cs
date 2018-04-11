using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

/// <summary>
/// This class inherits from "SteamVR_TrackedController" script.
/// It signals the game manager.
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
	
}
