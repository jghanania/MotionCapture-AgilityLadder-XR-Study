using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using static SettingsManager;

public class AgilityLadderController : MonoBehaviour
{
    public SettingsManager Settings;
    public ObjectManager Objects;

    public GameObject CurrentPath;
    public GameObject Path1;
    public GameObject Path2;
    public GameObject Path3;

    // variables for current field, previous field coordinates and the calculated direction
    private int currentIndex = 0;
    private int previousRow = 0;
    private int previousColumn = 3;
    private string stepDirection;

    // ladder rotation for potentiel correction of coordinates system
    private Quaternion ladderRotationVector;

    private void Start()
    {
        // In case the ladder is not perfectly parallel to the x and z axis, get it's rotation angle to rotate the coordinate system of the fields
        float ladderRotationAngle = Objects.AgilityLadder.transform.eulerAngles.y;
        // prepare rotation vector base on the ladder rotation
        ladderRotationVector = Quaternion.Euler(0, -ladderRotationAngle, 0);

        Settings.PathSetting = PathOptions.Path1;
        CurrentPath = Path1;
        ResetPath();

    }

    public void ResetPath()
    {
        ActivateChildrenObjects(Path1, false);
        ActivateChildrenObjects(Path2, false);
        ActivateChildrenObjects(Path3, false);
        CurrentPath = GameObject.Find(Settings.PathSetting.ToString());
        Objects.AgilityLadderPathBack.SetActive(true);
        currentIndex = 0;
        previousRow = 0;
        previousColumn = 3;
    }

    // Show the next field of the current Path
    public void NextField()
    {
        CurrentPath = GameObject.Find(Settings.PathSetting.ToString());

        if (CurrentPath == null)
        {
            Debug.LogError($"Target object '{Settings.PathSetting}' not found.");
            return;
        }

        Transform[] children = CurrentPath.GetComponentsInChildren<Transform>(true);

        if (currentIndex > 0 && currentIndex <= children.Length - 1)
        {
            Vector3 contactPointLeftToes = ladderRotationVector * (Objects.SubjectAvatarLeftFoot.transform.Find("leftToesSphere").position - children[currentIndex].transform.position) * 100;
            Vector3 contactPointLeftHeel = ladderRotationVector * (Objects.SubjectAvatarLeftFoot.transform.Find("leftHeelSphere").position - children[currentIndex].transform.position) * 100;
            Vector3 contactPointRightToes = ladderRotationVector * (Objects.SubjectAvatarRightFoot.transform.Find("rightToesSphere").position - children[currentIndex].transform.position) * 100;
            Vector3 contactPointRightHeel = ladderRotationVector * (Objects.SubjectAvatarRightFoot.transform.Find("rightHeelSphere").position - children[currentIndex].transform.position) * 100;

            float leftFootRotationAngle = Mathf.Atan2(-(contactPointLeftToes.x - contactPointLeftHeel.x), (contactPointLeftToes.z - contactPointLeftHeel.z)) * (180 / Mathf.PI) + 90;

            // right foot angle negativ to mirror compared to left
            float rightFootRotationAngle = -(Mathf.Atan2(-(contactPointRightToes.x - contactPointRightHeel.x), (contactPointRightToes.z - contactPointRightHeel.z)) * (180 / Mathf.PI) + 90);

            var match = Regex.Match(children[currentIndex].gameObject.name, @"^\w+ \((\d+)\) \((\d+)\)$");

            if (match.Success)
            {
                // Extract the row and column numbers and check direction of the step
                int column = int.Parse(match.Groups[1].Value);
                int row = int.Parse(match.Groups[2].Value);
                if (currentIndex != 0)
                {
                    if (column > previousColumn)
                    {
                        stepDirection = "right";
                        previousColumn = column;
                        previousRow = row;
                    }
                    else if (column < previousColumn)
                    {
                        stepDirection = "left";
                        previousColumn = column;
                        previousRow = row;
                    }
                    else if (row > previousRow)
                    {
                        stepDirection = "forward";
                        previousColumn = column;
                        previousRow = row;
                    }
                    else
                    {
                        Debug.LogWarning("something went wrong, step direction doesn't make sense");
                    }

                }

                // Log the data to a .csv file
                Settings.LogToCSV(row, column, stepDirection, contactPointLeftToes, contactPointLeftHeel, contactPointRightToes, contactPointRightHeel, leftFootRotationAngle, rightFootRotationAngle);
            }
        }

        // Make sure there are children to activate
        if (currentIndex < children.Length - 1)
        {
            // Activate the next child
            currentIndex++;
            Objects.AgilityLadderPathBack.SetActive(false);
            children[currentIndex].gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("No more Fields to activate.");
            ActivateChildrenObjects(CurrentPath, false);
            Objects.AgilityLadderPathBack.SetActive(true);
            currentIndex = 0;
            previousRow = 0;
            previousColumn = 3;

            if (Settings.PathSetting.ToString() == "Path1")
            {
                Settings.PathSetting = PathOptions.Path2;
            }
            else if (Settings.PathSetting.ToString() == "Path2")
            {
                Settings.PathSetting = PathOptions.Path3;
            }
            else if (Settings.PathSetting.ToString() == "Path3")
            {
                Debug.LogWarning("Finished");
                Settings.PathSetting = PathOptions.Path1;
            }

        }
    }

    // Show or hide all the children Object of a selected parentObject
    public void ActivateChildrenObjects(GameObject parentObject, Boolean activationState)
    {
        foreach (Transform childObject in parentObject.transform)
        {
            childObject.gameObject.SetActive(activationState);
        }
    }

}
