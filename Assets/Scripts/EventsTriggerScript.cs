using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsTriggerScript : MonoBehaviour {
    public GameManager gameManager;
    public bool waitingforResponse;
    // Use this for initialization
    void Start () {
        waitingforResponse = false;
	}
	
	// Update is called once per frame
	void Update () {
        if(gameManager.generate_state && GameObject.Find("TextStimulus(Clone)")==null && !waitingforResponse)
        {
            EventManager.TriggerEvent("spawnStim");
            waitingforResponse = true;
        }

        if (waitingforResponse)
        {
            
            //Debug.Log("waiting for keyboard input");
            if (Input.anyKeyDown)
            {
				EventManager.TriggerEvent("GetKeyResponse");
               // Debug.Log(Input.inputString);
                waitingforResponse = false;
            }
        }
	}
}
