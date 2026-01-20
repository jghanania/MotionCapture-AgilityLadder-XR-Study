using System.Collections;
using UnityEngine;
using static SettingsManager;

public class OptitrackController : MonoBehaviour
{
    public SettingsManager Settings;
    public ObjectManager Objects;

    public float AvatarScale;


    public enum ShowMarkerOptions
    {
        Show,
        Hide
    }

    public ShowMarkerOptions ShowMarker;

    void Start()
    {
        AvatarScale = PlayerPrefs.GetFloat("SavedAvatarScale", 1);
        ScaleAvatar();
    }


    void OnApplicationQuit()
    {
        // Save the AvatarScale when the game ends to avoid having to scale it again on each restart
        PlayerPrefs.SetFloat("SavedAvatarScale", AvatarScale);
        PlayerPrefs.Save();
    }


    // Function to show or hide Optitrack Marker in Scene
    public void ToggleOptiTrackMarker()
    {
        if (ShowMarker == ShowMarkerOptions.Show)
        {
            Objects.OptitrackClient.GetComponent<OptitrackStreamingClient>().DrawMarkers = true;
        }
        else if (ShowMarker == ShowMarkerOptions.Hide)
        {
            Objects.OptitrackClient.GetComponent<OptitrackStreamingClient>().DrawMarkers = false;
        }
        else
        {
            Debug.Log("ShowMarker enum state not valid");
        }
    }


    // Set the size of Avatar. Necessary for correct placement of optitracked skeleton
    public void ScaleAvatar()
    {
        Objects.SubjectAvatar.transform.parent.transform.localScale = new Vector3(AvatarScale, AvatarScale, AvatarScale);
    }



    // Function to align the cam with the optitrack skeleton trackers on the head at the start of the game
    public IEnumerator AlignCam()
    {
        Objects.OptitrackClient.GetComponent<OptitrackStreamingClient>().DrawMarkers = true;
        yield return new WaitForSeconds(0.1f);

        if (Objects.Camera == null || Objects.CameraRig == null)
        {
            Debug.LogWarning("Camera not found");
        }
        else
        {
            Debug.Log("camera found");
        }

        // Find the first instance of OptitrackSkeletonAnimator in the scene
        OptitrackSkeletonAnimator animator = FindAnyObjectByType<OptitrackSkeletonAnimator>();

        string OptitrackSkeletonID = "";
        if (animator != null)
        {
            // Access the GameObject that has the script
            OptitrackSkeletonID = animator.SkeletonAssetName;
            Debug.Log("Found OptiTrackID: " + OptitrackSkeletonID);
        }
        else
        {
            Debug.LogError("No GameObject with OptitrackSkeletonAnimator found in the scene.");
        }

        // Get skeleton ID from optitrack
        //string OptitrackSkeletonID = participantAvatar.GetComponent<OptitrackSkeletonAnimator>().SkeletonAssetName;

        // Find the two marker on the head of the avatar (always 14 and 13 in optitrack baseline skeleton templeate, other templates might vary)
        GameObject foreheadMarker = FindMarker(OptitrackSkeletonID, "14");
        GameObject topOfHeadMarker = FindMarker(OptitrackSkeletonID, "13");


        // Directional vector between forehead and top of head markers
        Vector3 direction = foreheadMarker.transform.position - topOfHeadMarker.transform.position;

        direction = new(direction.x, 0, direction.z);

        Debug.Log($"Head direction is {direction}");


        // cameraRig.transform.eulerAngles = new Vector3(0, 0, 0);
        // Get the current forward direction of the camera
        Vector3 cameraForward = Objects.Camera.transform.forward;

        Debug.Log($"Camera is looking {cameraForward}");

        // Calculate the angle between the camera's forward direction and the direction vector (from forehead to top of head)
        float angle = Vector3.SignedAngle(cameraForward, direction, Vector3.up);

        Debug.Log($"Angle between the two is {angle}");

        // Rotate the cameraRig around the Y-axis by the angle
        Quaternion rotation = Quaternion.Euler(0f, angle, 0f);

        Debug.Log($"Necessary rotation is {rotation}");

        Objects.CameraRig.transform.rotation *= rotation; // Apply the rotation to the cameraRig's current rotation

        Debug.Log($"Camera rotation is {Objects.Camera.transform.rotation.eulerAngles}");


        Vector3 translationVector = foreheadMarker.transform.position - Objects.Camera.transform.position;

        Objects.CameraRig.transform.position += translationVector - new Vector3(0f, 0.1f, 0f);

        Debug.Log($"Camera positon is {Objects.Camera.transform.position}");


        // Hide Marker once Cam is placed
        Objects.OptitrackClient.GetComponent<OptitrackStreamingClient>().DrawMarkers = false;
    }


    // Find the a specific marker of a skeleton
    public GameObject FindMarker(string OptitrackSkeletonID, string markerID)
    {

        GameObject marker = GameObject.Find($"({OptitrackSkeletonID}  Marker: {markerID})");
        if (marker == null)
        {
            marker = GameObject.Find($"Passive ({OptitrackSkeletonID} Member ID: {markerID})");
        }
        if (marker == null)
        {
            Debug.LogWarning("top of head marker not found");
            return null;
        }
        return marker;
    }
}
