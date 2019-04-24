using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;


public class Parameters
{
    public Experiment Experiment_Params { get; set; }
    public Stimulus Stimulus_Params { get; set; }
    // to add a new bloc of parameters, create a new class and a new level in the Parameters class to unpack them, making sure to tag the variables in the new bloc with the
    // JsonProperty flag, to enable them to be static in the program. (see https://stackoverflow.com/questions/22605529/deserializing-static-properties-using-json-net for more)
    //
    //Example JSON for this is:
    //    {
    //  "Experiment_Params": 
    //    {
    //      "Staircase_ratio": "321",
    //      "Trials": "20",
    //      "x_offset": "12",
    //      "y_offset": "123",
    //      "z_offset": "1234"
    //    }
    //  ,
    //  "Stimulus_Params": 
    //    {
    //      "Letter": "E",
    //      "Height": "0.5"
    //    }
    //}
}

public class Experiment
{
    // Use the JsonProperty tag to make these global variables, accessible from any script. 
	// Variable name must match its tag's name (lower upper case does not matter).
    [JsonProperty("InputMethod")]
    public static string InputMethod { get; set; }
    [JsonProperty("SaveLocation")]
    public static string SaveLocation { get; set; }
    [JsonProperty("Staircase")]
    public static float Staircase { get; set; }
    [JsonProperty("Num_Levels")]
    public static int Num_Levels { get; set; }
    [JsonProperty("Trials")]
    public static int Trials { get; set; }
    [JsonProperty("x_offset")]
    public static float X_offset { get; set; }
    [JsonProperty("y_offset")]
    public static float Y_offset { get; set; }
    [JsonProperty("z_offset")]
    public static float Z_offset { get; set; }
    [JsonProperty("X_Fixation")]
    public static float X_Fixation { get; set; }
    [JsonProperty("Y_Fixation")]
    public static float Y_Fixation { get; set; }
    [JsonProperty("Z_Fixation")]
    public static float Z_Fixation { get; set; }

}

public class Stimulus
{
    [JsonProperty("Type")]
    public static string Type { get; set; }
    [JsonProperty("Letter")] //make sure these are separated by a comma in your JSON file
    public static string Letter { get; set; }
    [JsonProperty("Height")]
    public static float Height { get; set; }
    [JsonProperty("Max_Color")]
    public static float Max_Color { get; set; }
    [JsonProperty("Duration")]
    public static float Duration { get; set; }
    [JsonProperty("GazeContingent")]
    public static bool GazeContingent { get; set; }

}

/*
// Comparison article: https://jacksondunstan.com/articles/3714
// JsonUtility does not support properties, {get; set;} need to be removed,
// [System.Serializable] need to be added.

[System.Serializable]
public class Parameters{
	public Experiment Experiment_Params;
	public Stimulus Stimulus_Params;
}

[System.Serializable]
public class Experiment{
	public float Staircase_ratio;
	public int Trials;
	public float x_offset;
	public float y_offset;
	public float z_offset;
}

[System.Serializable]
public class Stimulus{
	public string Letter;
	public float Height;
	public string Color;
}

void start(){
	Parameters allParam = new Parameters();
	string dataAsJson = File.ReadAllText(filePath); 
	allParam = JsonUtility.FromJson<Parameters>(dataAsJson);
	Debug.Log("Staircase ratio: "+ allParam.Experiment_Params.Staircase_ratio);
}
*/

public class GetConfig: MonoBehaviour
{

    private static string configFileName = "stimulusconfig.json";
    
    public void LoadParams()
    {   
        string filePath = Path.Combine(Application.dataPath, configFileName);

        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            Parameters all_params = JsonConvert.DeserializeObject<Parameters>(dataAsJson);
            all_params = null;
            //Uncomment below to view output in Unity console
            //Debug.Log(Experiment.Staircase);
        }

    }
    void Awake()
    {
        LoadParams();
    }
}



