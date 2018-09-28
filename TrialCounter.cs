using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrialCounter : MonoBehaviour {
    SMI.SMIEyeTrackingUnity smiInstance = null;
    public GameManager gameManager = GameManager.instance;
    public Vector3 visualFieldLocation;
    public TextMesh count;
    // Use this for initialization
    private void Awake()
    {
        //gameManager = GameManager.instance;
    }
    void Start () {
        visualFieldLocation.Set(-15, 15, 0);
        smiInstance = SMI.SMIEyeTrackingUnity.Instance;
        //count = GetComponent<TextMesh>();
    }
	
	// Update is called once per frame
	void Update () {
        count.text = gameManager.trial_number.ToString();
        count.characterSize = 1;
        transform.LookAt(smiInstance.transform);
        transform.rotation *= Quaternion.Euler(0, 180, 0);
        transform.position =   smiInstance.transform.position + Quaternion.Euler(visualFieldLocation) *smiInstance.transform.forward* Stimulus.StimDepth;
	}
}
