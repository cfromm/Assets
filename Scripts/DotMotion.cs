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
	// Use this for initialization

	void Start () {

        speedApertureUnits = Stimulus.DotSpeed/(2*Stimulus.ApertureRad);// Aperture has scaled radius of 1 here, need to scale speed to use with local position
        //Debug.Log("a: " + transform.parent.localScale);

        if (current_direction == 0) //right motion
        {
            movement_angle = Quaternion.AngleAxis(-Random.Range(-current_angle, current_angle), Vector3.up);
            //movement_angle = Quaternion.AngleAxis(45f, Vector3.up);
            transform.localRotation = Quaternion.identity * movement_angle;
            //Debug.Log("movement angle is " + movement_angle.eulerAngles);
        }
        else //left motion
        {
            movement_angle = Quaternion.AngleAxis(Random.Range(-current_angle, current_angle), Vector3.up);
            //movement_angle = Quaternion.AngleAxis(45f, Vector3.up); ;
            transform.localRotation = Quaternion.identity * movement_angle;
           // Debug.Log("movement angle is " + movement_angle.eulerAngles);
        }
        //GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        
        //movement_direction = (movement_angle * transform.right);// + transform.localPosition;
        //Debug.Log("Magnitude of move is:" + (movement_direction - transform.localPosition).magnitude);
        //float angle = Vector3.Angle(transform.right, movement_direction);
        //Debug.Log("dot moving with angle of " + movement_angle.eulerAngles);
        //Debug.Log("Angle between is: " + angle);
        timeSinceStartup = Time.realtimeSinceStartup;
    }


	// Update is called once per frame
	void Update () {
        
        
        if ((Time.realtimeSinceStartup) < Stimulus.Duration + start_of_stimulus)
        {
            gameObject.SetActive(true);
            
            if ((Time.realtimeSinceStartup) < Stimulus.DotLife + start_of_dot)
            {
                float deltaT = 1 / 90f; //change this after debugging

                transform.Translate(Vector3.right * speedApertureUnits * Time.deltaTime* transform.parent.localScale.x, Space.Self);
                //Debug.Log("b: " + transform.parent.localScale);
               // if (current_direction == 1)
                //{ //transform.localPosition = -Vector3.MoveTowards(transform.localPosition, movement_direction, (speed * Time.deltaTime));
                    
                    
                //}
                // transform.position += moveAng*(Vector3.forward*speed)
                //else {
                    //transform.localPosition = Vector3.MoveTowards(transform.localPosition, movement_direction, (speed * Time.deltaTime));
                //   transform.Translate(Vector3.right  * speed* deltaT, Space.Self); 
                //}

                



                if (transform.localPosition.magnitude >= 0.5f)
                {
                    float old_x = transform.localPosition.x;
                    float old_z = transform.localPosition.z;
                    gameObject.SetActive(false);
                    transform.localPosition = new Vector3(-old_x, 0, -old_z);
                    //movement_angle = Quaternion.Euler(0, Random.Range(0f, current_angle), 0); ;
                    //movement_direction = (movement_angle * transform.right) + transform.localPosition;
                    gameObject.SetActive(true);
                }
            }
            else
            {
               // Debug.Log("Respawning dot");
                gameObject.SetActive(false);
                Vector2 dot_position = Random.insideUnitCircle * 0.5f;
                transform.localPosition = new Vector3(dot_position[0], 0, dot_position[1]);
                //movement_angle = Quaternion.Euler(0, Random.Range(0f, current_angle), 0);
                //movement_direction = (movement_angle * transform.right) + transform.localPosition;
                start_of_dot = Time.realtimeSinceStartup;
                gameObject.SetActive(true);
            }
            
        }
        else { gameObject.SetActive(false); }

        if (Time.realtimeSinceStartup > start_of_stimulus + 9f)
        {
            Debug.Log("Dot is "+ transform.localPosition.magnitude +" away from center");
            Debug.Break();
        }
	}
}
