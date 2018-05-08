using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script should be attached to type 'p' stimulus's object.
/// Detects trigger collision and invokes corresponding method.
/// </summary>
public class TestCollide : MonoBehaviour {

    private ResponseGetter response_script;
	
	private void OnEnable()
    {
		EventManager.StartListening("DestroyStim", DestroyStim);
    }

    private void OnDisable()
    {
		EventManager.StopListening("DestroyStim", DestroyStim);
    }

    /// <summary>
    /// Creates a box collider for the object to detect ray pointer.
    /// </summary>
    public void Start()
	{
		GameObject response_obj = GameObject.Find("ResponseModule");
		response_script = response_obj.GetComponent<ResponseGetter>();
		
		if(this.gameObject.GetComponent<BoxCollider>() == null)
		{
			this.gameObject.AddComponent<BoxCollider>();
		}	
	}

    /// <summary>
    /// This function tells ResponseGetter the user points at it.
    /// </summary>
    public void OnTriggerEnter(Collider collision)
	{
		//Debug.Log("OnTriggerEnter");		
		response_script.SetPointResponse(true);
	}

    /// <summary>
    /// This function tells ResponseGetter the user leaves it.
    /// </summary>
    public void OnTriggerExit(Collider collision)
	{
		//Debug.Log("OnTriggerExit");		
		response_script.SetPointResponse(false);
	}
	
	/// <summary>
	/// This function destroys the current stimulus instantly.
	/// </summary>
	private void DestroyStim()
	{		
		Destroy( gameObject );		
	}
}
