using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotMotion : MonoBehaviour {
    public float current_angle; //assigned by parent, chosen randomly from list in JSON
    public float current_direction; //assigned by parent, chosen randomly as 0 or 1 (left, right)
    //public Vector3 destination;
    public Quaternion movement_angle;
    public Vector3 movement_end;
    public float speedApertureUnits;
    public float start_of_dot;
    public float start_of_stimulus;
    public float timeSinceStartup;
    public bool isNoise;
	// Use this for initialization

	void Start () {

        speedApertureUnits = Stimulus.DotSpeed/(2*Stimulus.ApertureRad);// Aperture has scaled radius of 1 here, need to scale speed to use with local position


        if (current_direction == 1 && !isNoise) //left motion
        {
            movement_angle = Quaternion.AngleAxis(-Random.Range(-current_angle/2, current_angle/2), Vector3.up);
            transform.localRotation = Quaternion.identity * movement_angle;
            //Debug.Log("movement angle is " + movement_angle.eulerAngles);
        }
        if( current_direction != 1 && ! isNoise) //right motion
        {
            movement_angle = Quaternion.AngleAxis(Random.Range(-current_angle/2, current_angle/2), Vector3.up);
            transform.localRotation = Quaternion.identity * movement_angle;
           // Debug.Log("movement angle is " + movement_angle.eulerAngles);
        }
        if (isNoise) 
        {
            movement_angle = Quaternion.AngleAxis(Random.Range(-360, 360), Vector3.up);
            transform.localRotation = Quaternion.identity * movement_angle;
        }
        //GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        
        timeSinceStartup = Time.realtimeSinceStartup;
    }



	void Update () {
        
        
        if ((Time.realtimeSinceStartup) < Stimulus.Duration + start_of_stimulus)
        {
            gameObject.SetActive(true);
            
            if ((Time.realtimeSinceStartup) < Stimulus.DotLife + start_of_dot)
            {
                float deltaT = 1 / 90f; //change this after debugging
                if (current_direction == 1)
                {
                    transform.Translate(Vector3.right * speedApertureUnits * Time.deltaTime * transform.parent.localScale.x, Space.Self);

                }
                else
                {
                    transform.Translate(-Vector3.right * speedApertureUnits * Time.deltaTime * transform.parent.localScale.x, Space.Self);
                }
                if (transform.localPosition.magnitude >= 0.5f)
                {
                    float old_x = transform.localPosition.x;
                    float old_z = transform.localPosition.z;
                    gameObject.SetActive(false);
                    transform.localPosition = new Vector3(-old_x, 0, -old_z);
                    gameObject.SetActive(true);
                }
            }
            else
            {
               // Debug.Log("Respawning dot");
                gameObject.SetActive(false);
                Vector2 dot_position = Random.insideUnitCircle * 0.5f;
                transform.localPosition = new Vector3(dot_position[0], 0, dot_position[1]);
                start_of_dot = Time.realtimeSinceStartup;
                gameObject.SetActive(true);
            }
            
        }
        else { gameObject.SetActive(false); }

        //if (Time.realtimeSinceStartup > start_of_stimulus + 9f)
        //{
        //    Debug.Log("Dot is "+ transform.localPosition.magnitude +" away from center");
        //    Debug.Break();
        //}
	}
}
