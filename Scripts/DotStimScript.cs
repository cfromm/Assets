using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotStimScript : MonoBehaviour {
    //When the dot stimulus is instantiated, it calls this to create all the dots it needs as children.
    public int num_dots = Mathf.RoundToInt(Mathf.Pow(Stimulus.ApertureRad, 2) * Mathf.PI * Stimulus.Density);

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < num_dots; i++)
        {
            GameObject dot = (GameObject)Instantiate(Resources.Load("SingleDot"));
            dot.transform.parent = transform;
            dot.transform.localPosition = new Vector3(Random.Range(0, Stimulus.ApertureRad), Random.Range(0, Stimulus.ApertureRad), Random.Range(0, Stimulus.ApertureRad));    
        }
    }
	// Update is called once per frame

}
