using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotMotion : MonoBehaviour {
    public float current_angle;
    public float current_direction;
    public Vector3 destination;
    public Vector3 movement_angle;
    private float speed;
    public float start_of_dot;
	// Use this for initialization
	void Start () {
        if (current_direction == 0)
        { current_angle = -1 * current_angle; }
        speed = Stimulus.DotSpeed / Stimulus.ApertureRad;// Aperture has scaled radius of 1 here, need to scale speed to use with local position
        movement_angle = Quaternion.Euler(Random.Range(0f, current_angle), 0, 0) * transform.forward;
        Debug.Log("dot moving with max angle of " + movement_angle);
        destination = movement_angle + new Vector3( speed * Stimulus.DotLife, 0, 0);

    }


	// Update is called once per frame
	void Update () {
        
		if (transform.localPosition.magnitude < 0.5f && (Time.realtimeSinceStartup - start_of_dot) > Stimulus.DotLife)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, destination, (speed * Stimulus.DotLife));
            //Debug.Log(destination);
        }
        if (transform.localPosition.magnitude >= 0.5f && (Time.realtimeSinceStartup - start_of_dot) > Stimulus.DotLife)
        {
            gameObject.SetActive(false);
            Vector2 new_dot_position = Random.insideUnitCircle * 0.5f;
            transform.localPosition = new Vector3(new_dot_position[0], 0, new_dot_position[1]);
            gameObject.SetActive(true);
        }
        if((Time.realtimeSinceStartup - start_of_dot) > Stimulus.DotLife)
            
        {
            gameObject.SetActive(false);
        }
	}
}
