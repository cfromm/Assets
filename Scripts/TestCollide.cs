using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script should be attached to type 'p' stimulus's object.
/// </summary>
public class TestCollide : MonoBehaviour {
	
	private ResponseGetter response_script;
	
	public void OnEnable()
    {
		EventManager.StartListening("DestroyStim", DestroyStim);
    }
    public void OnDisable()
    {
		EventManager.StopListening("DestroyStim", DestroyStim);
    }
	
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
    void OnTriggerEnter(Collider collision)
	{
		//Debug.Log("OnTriggerEnter");		
		response_script.GetPointResponse(true);
	}

    /// <summary>
    /// This function tells ResponseGetter the user leaves it.
    /// </summary>
    void OnTriggerExit(Collider collision)
	{
		//Debug.Log("OnTriggerExit");		
		response_script.GetPointResponse(false);
	}
	
	/// <summary>
	/// This function destroys the current stimulus instantly
	/// </summary>
	private void DestroyStim()
	{		
		Destroy( gameObject );		
	}
}
