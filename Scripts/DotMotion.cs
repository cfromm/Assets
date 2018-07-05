using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotMotion : MonoBehaviour {
    public float current_angle; //assigned by parent, chosen randomly from list in JSON
    public float current_direction; //assigned by parent, chosen randomly as 0 or 1 (left, right)
    //public Vector3 destination;
    public Quaternion movement_angle;
    public Vector3 movement_direction;
    public float speed;
    public float start_of_dot;
    public float start_of_stimulus; 
	// Use this for initialization

	void Start () {

        speed =  (2*Stimulus.ApertureRad)/Stimulus.DotSpeed ;// Aperture has scaled radius of 1 here, need to scale speed to use with local position
        movement_angle = Quaternion.Euler(0, Random.Range(0f, current_angle), 0);

        //Debug.Log("dot moving with angle of " + movement_angle.eulerAngles);
        //destination = movement_angle * new Vector3(speed * Stimulus.DotLife, 0, 0);

    }


	// Update is called once per frame
	void Update () {
        

        if ((Time.realtimeSinceStartup) < Stimulus.Duration + start_of_stimulus)
        {
            gameObject.SetActive(true);
            if ((Time.realtimeSinceStartup) < Stimulus.DotLife + start_of_dot)
            {
                if (current_direction == 1)
                { transform.Translate(movement_angle * -Vector3.right * (speed / 90f), Space.Self); }
                else { transform.Translate(movement_angle * Vector3.right * (speed / 90f), Space.Self); }

                //transform.localPosition = Vector3.MoveTowards(transform.localPosition, movement_direction, (speed * 1/90f));



                if (transform.localPosition.magnitude >= 0.5f)
                {
                    float old_x = transform.localPosition.x;
                    float old_z = transform.localPosition.z;
                    gameObject.SetActive(false);
                    transform.localPosition = new Vector3(-old_x, 0, old_z);
                    movement_angle = Quaternion.Euler(0, Random.Range(0f, current_angle), 0); ;
                    gameObject.SetActive(true);
                }
            }
            else
            {
                Debug.Log("Respawning dot");
                gameObject.SetActive(false);
                Vector2 dot_position = Random.insideUnitCircle * 0.5f;
                transform.localPosition = new Vector3(dot_position[0], 0, dot_position[1]);
                movement_angle = Quaternion.Euler(0, Random.Range(0f, current_angle), 0);
                start_of_dot = Time.realtimeSinceStartup;
                gameObject.SetActive(true);
            }
            
        }
        else { gameObject.SetActive(false); }

	}
}
