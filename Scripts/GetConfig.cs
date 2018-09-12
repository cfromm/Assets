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
    [JsonProperty("SubjectInitials")]
    public static string SubjectIntials { get; set; }
    [JsonProperty("InputMethod")]
    public static string InputMethod { get; set; }
    [JsonProperty("SaveBool")]
    public static bool SaveBool { get; set; }
    [JsonProperty("WriteTrialData")]
    public static bool WriteTrialData { get; set; }
    [JsonProperty("WriteFrameData")]
    public static bool WriteFrameData { get; set; }
    [JsonProperty("SaveLocation")]
    public static string SaveLocation { get; set; }
    [JsonProperty("Staircase_ratio")]
    public static float Staircase { get; set; }
    [JsonProperty("Num_Levels")]
    public static int Num_Levels { get; set; }
    [JsonProperty("Trials")]
    public static int Trials { get; set; }
    [JsonProperty("BackgroundColor")]
    public static string BackgroundColor { get; set; }
    [JsonProperty("vertical_offset")]
    public static float X_offset { get; set; }
    [JsonProperty("horizontal_offset")]
    public static float Y_offset { get; set; }
    [JsonProperty("z_offset")]
    public static float Z_offset { get; set; }
    [JsonProperty("X_Fixation")]
    public static float X_Fixation { get; set; }
    [JsonProperty("Y_Fixation")]
    public static float Y_Fixation { get; set; }
    [JsonProperty("Z_Fixation")]
    public static float Z_Fixation { get; set; }
    [JsonProperty("FixationRadiusDeg")]
    public static float FixationRad { get; set; }
    [JsonProperty("ZoneOfFixation")]
    public static float Fixation_zone { get; set; }
    [JsonProperty("Fixation_Roving")]
    public static bool Fixation_Roving { get; set; }
    [JsonProperty("Roving_outer_vertical")]
    public static float Roving_outer_vertical { get; set; }
    [JsonProperty("Roving_outer_horizontal")]
    public static float Roving_outer_horizontal { get; set; }
    [JsonProperty("Roving_step")]
    public static float Roving_step { get; set; }
    [JsonProperty("Monocular")]
    public static string Monocular { get; set; }
    [JsonProperty("Total_Staircases")]
    public static int Num_Staircases { get; set; }
    [JsonProperty("Staricase1_Initial")]
    public static int Stair1_Init { get; set; }
    [JsonProperty("Staricase2_Initial")]
    public static int Stair2_Init { get; set; }
    [JsonProperty("Staricase3_Initial")]
    public static int Stair3_Init { get; set; }
    [JsonProperty("InputPadAxis")]
    public static string InputAxis { get; set; }

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
    [JsonProperty("FixationDurationRequired")]
    public static float FixationDuration { get; set; }
    [JsonProperty("Angle")]
    public static string Angle { get; set; }
    [JsonProperty("ApertureRad")]
    public static float ApertureRad { get; set; }
    [JsonProperty("InterTrialInterval")]
    public static float ITI { get; set; }
    [JsonProperty("Density")]
    public static float Density { get; set; }
    [JsonProperty("PctNoiseDots")]
    public static float PctNoiseDots { get; set; }
    [JsonProperty("SizeArcmin")]
    public static float DotSize { get; set; }
    [JsonProperty("SpeedInDegreesPerSec")]
    public static float DotSpeed { get; set; }
    [JsonProperty("IndividualLifetime")]
    public static float DotLife { get; set; }
    [JsonProperty("Directions")]
    public static string Directions { get; set; }
    [JsonProperty("2Dor3D")]
    public static int FlatOrRound { get; set; }
    [JsonProperty("StimDepthMeters")]
    public static float StimDepth { get; set; }
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
            //TextAsset dataAsJsonObj = Resources.Load<TextAsset>(configFileName);
            //string dataAsJson = dataAsJsonObj.text;

            Debug.Log(dataAsJson);
            //string dataAsJson = Resources.Load(configFileName);
            Parameters all_params = JsonConvert.DeserializeObject<Parameters>(dataAsJson);
            all_params = null;
            //Uncomment below to view output in Unity console
            Debug.Log("Stimulus type: " + Stimulus.Type);
        }

    }
    void Awake()
    {
        LoadParams();
    }
}



