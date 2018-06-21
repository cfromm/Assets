using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotMotion : MonoBehaviour {
    public float current_angle; //assigned by parent, chosen randomly from list in JSON
    public float current_direction; //assigned by parent, chosen randomly as 0 or 1 (left, right)
    public Vector3 destination;
    public Vector3 movement_angle;
    public float speed;
    public float start_of_dot;
	// Use this for initialization

	void Start () {

        speed = Stimulus.DotSpeed / Stimulus.ApertureRad;// Aperture has scaled radius of 1 here, need to scale speed to use with local position
        movement_angle = Quaternion.Euler(0, Random.Range(0f, current_angle), 0) * transform.right;
        if (current_direction == 1)
        { movement_angle = -1 * movement_angle; }
        //Debug.Log("dot moving with angle of " + movement_angle.eulerAngles);
        //destination = movement_angle * new Vector3(speed * Stimulus.DotLife, 0, 0);

    }


	// Update is called once per frame
	void Update () {
        
		if ((Time.realtimeSinceStartup) < Stimulus.DotLife + start_of_dot)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, movement_angle, (speed * 1/90f));

            if (transform.localPosition.magnitude >= 0.5f)
            {
                
                gameObject.SetActive(false);
                Vector2 new_dot_position = Random.insideUnitCircle * 0.5f;
                transform.localPosition = new Vector3(new_dot_position[0], 0, new_dot_position[1]);
                gameObject.SetActive(true);
            }
        }
 
        else
            
        {
            gameObject.SetActive(false);
        }
	}
}
