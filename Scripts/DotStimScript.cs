using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotStimScript : MonoBehaviour {
    //When the dot stimulus is instantiated, it calls this to create all the dots it needs as children and set them moving.
    public float num_dots;
    public float dot_diam_units;
    public float ap_rad_units;
    public int stim_direction;
    public float max_angle;
    public float draw_time;
    List<GameObject> dots;

    void Start()
    {
        stim_direction = Random.Range(0, 2);
        num_dots = Mathf.Pow(Stimulus.ApertureRad, 2f) * Mathf.PI * Stimulus.Density;
        dot_diam_units = ((Stimulus.DotSize * Mathf.PI) / (60 * 180)) * Stimulus.StimDepth; //convert arcmin to radians (drop sin term due to small angle approx) and scale by depth
        ap_rad_units = ((Stimulus.ApertureRad * Mathf.PI) / (180)) * Stimulus.StimDepth;
        dots = new List<GameObject>();
        for (int i = 0; i < (int)num_dots; i++)
        {
            GameObject dot = (GameObject)Instantiate(Resources.Load("SingleDot"));
            dot.transform.parent = transform;
            Vector2 dot_position = Random.insideUnitCircle * 0.5f;
            dot.transform.localPosition = new Vector3(dot_position[0], 0, dot_position[1]);
            dot.transform.localScale = new Vector3(dot_diam_units/(2*ap_rad_units), 0, dot_diam_units/(2*ap_rad_units));
            dot.GetComponent<DotMotion>().current_angle = max_angle;
            dot.GetComponent<DotMotion>().current_direction = stim_direction;
            dot.SetActive(false);
            dots.Add(dot);
        }
        drawDots();
    }

    void drawDots()
    {

        for(int i = 0; i < (int)num_dots; i++)
        {
            if (!dots[i].activeInHierarchy)
            {
                dots[i].SetActive(true);
                dots[i].GetComponent<DotMotion>().start_of_dot = Time.realtimeSinceStartup;
            }
        }

    }
}
