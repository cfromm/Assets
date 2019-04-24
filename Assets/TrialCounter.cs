using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrialCounter : MonoBehaviour {
    SMI.SMIEyeTrackingUnity smiInstance = null;
    public GameManager gameManager = GameManager.instance;
    public Vector3 visualFieldLocation;
    public TextMesh count;
    public float running_pct_correct; 
    // Use this for initialization
    private void Awake()
    {
        //gameManager = GameManager.instance;
    }
    void Start () {
        visualFieldLocation.Set(15, 15, 0);
        smiInstance = SMI.SMIEyeTrackingUnity.Instance;
        //count = GetComponent<TextMesh>();
    }
	
	// Update is called once per frame
	void Update () {
        if (gameManager.trial_number != 0)
        {

            running_pct_correct = Mathf.Round((float)gameManager.total_correct /(float)gameManager.trial_number * 100);
        }
        else
        { running_pct_correct = 0; }

        if (Experiment.LevelOrPct == "l")
        { count.text = gameManager.current_level.ToString(); }
        else {count.text = running_pct_correct.ToString()+'%'; }
        
        if (gameManager.trial_success)
            {count.color = Color.gray; }
        else
        { count.color = Color.gray;  }
        count.characterSize = 1;
        transform.LookAt(smiInstance.transform);
        transform.localRotation = Quaternion.Euler(0, 0, 0); 
        transform.position =   smiInstance.transform.position + smiInstance.transform.rotation * Quaternion.Euler(visualFieldLocation) * Vector3.forward*Stimulus.StimDepth;

    }
}
