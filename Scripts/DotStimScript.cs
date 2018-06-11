using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotStimScript : MonoBehaviour {
    //When the dot stimulus is instantiated, it calls this to create all the dots it needs as children and set them moving.
    public float num_dots;
    public float dot_diam_units;
    public float ap_rad_units;
    public int stim_direction;

    void Start()
    {
        stim_direction = Random.Range(0, 2);
        num_dots = Mathf.Pow(Stimulus.ApertureRad, 2f) * Mathf.PI * Stimulus.Density;
        dot_diam_units = ((Stimulus.DotSize * Mathf.PI) / (60 * 180)) * Stimulus.StimDepth; //convert arcmin to radians (drop sin term due to small angle approx) and scale by depth
        ap_rad_units = ((Stimulus.ApertureRad * Mathf.PI) / (180)) * Stimulus.StimDepth;
        Debug.Log("Dots scale(absolute): " + dot_diam_units);
        for (int i = 0; i < (int)num_dots; i++)
        {
            GameObject dot = (GameObject)Instantiate(Resources.Load("SingleDot"));
            dot.transform.parent = transform;
            Vector2 dot_position = Random.insideUnitCircle * 0.5f;
            dot.transform.localPosition = new Vector3(dot_position[0], 0, dot_position[1]);
            dot.transform.localScale = new Vector3(dot_diam_units/(2*ap_rad_units), 0, dot_diam_units/(2*ap_rad_units));

        }
    }


}
