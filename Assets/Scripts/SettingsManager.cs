using Meta.XR.Depth;
using Meta.XR.MRUtilityKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using UnityEngine;


public class SettingsManager : MonoBehaviour
{
    public ObjectManager Objects;
    public OptitrackController OptitrackController;
    public AgilityLadderController AgilityLadderController;


    // Enums with the option for Environment, Visualization and Paths
    public enum EnvironmentOptions
    {
        VR,
        AR,
        RealWorld
    }
    public enum VisualizationOptions
    {
        Empty,
        Footsteps
    }
    public enum PathOptions
    {
        Path1,
        Path2,
        Path3
    }

    // Enum with Gender options
    public enum GenderOptions
    {
        Male,
        Female,
        Divers
    }

    // Participant Information for analysis purposes
    // Hidden, as buttons are defined in the custom Editor at /Assets/Editor
    [HideInInspector]
    public int SubjectID;
    [HideInInspector]
    public int SubjectHeight;
    [HideInInspector]
    public int SubjectShoeSize;
    [HideInInspector]
    public GenderOptions SubjectGender;
    [HideInInspector]
    public float AvatarScale;

    // Conditions Settings
    // Hidden, as buttons are defined in the custom Editor at /Assets/Editor
    [HideInInspector]
    public EnvironmentOptions EnvironmentSetting;
    [HideInInspector]
    public VisualizationOptions VisualizationSetting;
    public PathOptions PathSetting;
    [HideInInspector]
    public int ConditionNumber;


    private void Start()
    {
        // Activate avatar in case it isn't already and scale it to participant height
        Objects.SubjectAvatar.SetActive(true);


        // display starting state in the console
        Debug.Log($" Subject : {SubjectID,-4}" +
        $"            Environment   : {EnvironmentSetting,-10}" +
        $"            Visualization : {VisualizationSetting,-10}\n");
    }


    [HideInInspector]
    public readonly string[,] latinSquare = new string[6, 6]
    {
        { "RealWorld-Empty", "AR-Footsteps", "VR-Footsteps", "AR-Empty", "VR-Empty", "RealWorld-Footsteps" },
        { "AR-Footsteps", "AR-Empty", "RealWorld-Empty", "RealWorld-Footsteps", "VR-Footsteps", "VR-Empty" },
        { "AR-Empty", "RealWorld-Footsteps", "AR-Footsteps", "VR-Empty", "RealWorld-Empty", "VR-Footsteps" },
        { "RealWorld-Footsteps", "VR-Empty", "AR-Empty", "VR-Footsteps", "AR-Footsteps", "RealWorld-Empty" },
        { "VR-Empty", "VR-Footsteps", "RealWorld-Footsteps", "RealWorld-Empty", "AR-Empty", "AR-Footsteps" },
        { "VR-Footsteps", "RealWorld-Empty", "VR-Empty", "AR-Footsteps", "RealWorld-Footsteps", "AR-Empty" }
    };

    public void SetBalancedLatinSquare()
    {
        string[] parts;
        if (ConditionNumber == 0)
        {
            parts = new string[] { "VR", "Empty" };
        }
        else
        {
            int subjectIndex = (SubjectID - 1) % latinSquare.GetLength(0);
            int conditionIndex = ConditionNumber - 1;

            //Debug.Log($"Subject index {subjectIndex} and condition index {conditionIndex}");

            string entry = latinSquare[subjectIndex, conditionIndex];

            //Debug.Log($"{entry}");
            parts = entry.Split('-');
        }

        EnvironmentSetting = (EnvironmentOptions)System.Enum.Parse(typeof(EnvironmentOptions), parts[0]);
        VisualizationSetting = (VisualizationOptions)System.Enum.Parse(typeof(VisualizationOptions), parts[1]);
        SetEnvironment();
        SetVisualization();
    }


    public void SetSubjectGender()
    {
        if (SubjectGender == GenderOptions.Female)
        {
            Objects.SubjectAvatar = Objects.FemaleAvatar;
            Objects.FemaleAvatar.SetActive(true);
            Objects.MaleAvatar.SetActive(false);
        }
        else if (SubjectGender == GenderOptions.Male)
        {
            Objects.SubjectAvatar = Objects.MaleAvatar;
            Objects.FemaleAvatar.SetActive(false);
            Objects.MaleAvatar.SetActive(true);
        }
        OptitrackController.ScaleAvatar();
    }


    public void SetVisualization()
    {
        if (VisualizationSetting.ToString() == "Empty")
        {
            Objects.FieldMaterial.mainTexture = Objects.EmptyFieldTexture;
        }
        else if (VisualizationSetting.ToString() == "Footsteps")
        {
            Objects.FieldMaterial.mainTexture = Objects.FootstepsFieldTexture;
        }
    }


    public void SetEnvironment()
    {
        if (EnvironmentSetting.ToString() == "RealWorld")
        {
            Objects.Lab.SetActive(false);
            Objects.AvatarMeshRenderer.SetActive(true);
            Objects.OcclusionController.EnableOcclusionType(OcclusionType.NoOcclusion, updateDepthTextureProvider: true);
        }
        else if (EnvironmentSetting.ToString() == "VR")
        {
            Objects.Lab.SetActive(true);
            Objects.AvatarMeshRenderer.SetActive(true);
            Objects.OcclusionController.EnableOcclusionType(OcclusionType.NoOcclusion, updateDepthTextureProvider: true);
        }
        else if (EnvironmentSetting.ToString() == "AR")
        {
            Objects.AvatarMeshRenderer.SetActive(false);
            Objects.Lab.SetActive(false);
            Objects.OcclusionController.EnableOcclusionType(OcclusionType.SoftOcclusion);
        }
    }
    

    public void LogToCSV(int row, int column, string stepDirection, Vector3 contactPointLeftToes, Vector3 contactPointLeftHeel, Vector3 contactPointRightToes, Vector3 contactPointRightHeel, float leftFootRotationAngle, float rightFootRotationAngle)
    {
        string csvSeperator = ";";
        string currentDate = DateTime.Today.ToString("yyyy_MM_dd");

        // Define the file path
        string projectPath = Path.Combine(Application.dataPath, ".."); // Go up one level from Assets
        string folderPath = Path.Combine(projectPath, "LoggedData");
        Directory.CreateDirectory(folderPath); // Ensure folder exists
        string filePath = Path.Combine(folderPath, $"{currentDate}_Participant_{SubjectID}_Data.csv");

        // Open the file for writing
        using StreamWriter writer = new(filePath, true);

        long timestamp = System.DateTime.Now.Ticks;


        // Prepare the list of data for CSV (same variables as before)
        List<(string header, object value)> dataForCSV = new()
        {
            // Header data (no change from original)
            ("timestamp", timestamp),
            ("Subject", SubjectID),
            ("Gender", SubjectGender),
            ("BodyHeight", SubjectHeight),
            ("ShoeSize", SubjectShoeSize),
            ("Environment", EnvironmentSetting),
            ("Visualization", VisualizationSetting),
            ("Path", PathSetting),
            ("Column", column),
            ("Row", row),
            ("Direction", stepDirection),
        
            // Log the contact point coordinates and angles
            ("Left-Toes-X", 20 - contactPointLeftToes.z),
            ("Left-Toes-Z", 20 + contactPointLeftToes.x),
            ("Left-Heel-X", 20 - contactPointLeftHeel.z),
            ("Left-Heel-Z", 20 + contactPointLeftHeel.x),
            ("Right-Toes-X", 20 + contactPointRightToes.z),
            ("Right-Toes-Z", 20 + contactPointRightToes.x),
            ("Right-Heel-X", 20 + contactPointRightHeel.z),
            ("Right-Heel-Z", 20 + contactPointRightHeel.x),
        
            // Foot rotation angles
            ("Left-Foot-Angle", leftFootRotationAngle),
            ("Right-Foot-Angle", rightFootRotationAngle)
        };

        // Check if the file is new or empty, and write the header if it is
        if (writer.BaseStream.Length == 0)
        {
            // Write the CSV header, dynamically generated from the `dataForCSV` list
            writer.WriteLine(string.Join(csvSeperator, dataForCSV.Select(item => item.header)));
        }

        // Write the data row
        writer.WriteLine(string.Join(csvSeperator, dataForCSV.Select(item => item.value.ToString())));
    }


}