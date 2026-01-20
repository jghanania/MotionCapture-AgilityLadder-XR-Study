using Meta.XR.Depth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    [Header("Optitrack & Avatar")]
    public GameObject OptitrackClient;
    public GameObject SubjectAvatar;
    public GameObject FemaleAvatar;
    public GameObject MaleAvatar;
    public GameObject SubjectAvatarLeftFoot;
    public GameObject SubjectAvatarRightFoot;
    public GameObject AvatarMeshRenderer;

    [Header("Environments & Objects")]
    public GameObject Lab;
    public GameObject AgilityLadder;
    public GameObject AgilityLadderPathBack;
    public Material FieldMaterial;
    public Texture EmptyFieldTexture;
    public Texture FootstepsFieldTexture;

    [Header("Meta Quest")]
    public EnvironmentDepthOcclusionController OcclusionController;
    public GameObject CameraRig;
    public Camera Camera;
    public OVRPassthroughLayer OVRPassthroughLayer;

}
