using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WIM_Plugin {
    // Data describing the current WIM state. Data only. Modified at runtime.
    public class WIMData : ScriptableObject {
        public Transform WIMLevelTransform;
        public Transform PlayerRepresentationTransform;
        public Transform DestinationIndicatorInWIM;
        public Transform LevelTransform;
        public Transform PlayerTransform;


        public bool IsNewDestination { get; set; }
        public Transform DestinationIndicatorInLevel { get; set; }
        public GameObject TravelPreviewAnimationObj { get; set; }
        public GameObject TravelPreviewAnimationObj2 { get; set; }
        public Transform HMDTransform { get; set; }
        public Transform FingertipIndexR { get; set; }
        public Transform OVRPlayerController { get; set; }
        public bool ArmLengthDetected { get; set; }
        public PostTravelPathTrace PathTrace { get; set; }
        public Material PreviewScreenMaterial { get; set; }
        public float WIMHeightRelativeToPlayer { get; set; }
        public Vector3 WIMLevelLocalPosOnTravel { get; set; }
        public bool isNewDestination { get; set; }
        public Transform DestinationIndicatorInWIM2 { get; set; }
        public bool PreviewScreenEnabled { get; set; }
        public Vector3 WIMLevelLocalPos { get; set; }
        public bool PrevSemiTransparent { get; set; }
        public float PrevTransparency { get; set; }
    }







    //internal interface WIMData {
    //    bool IsNewDestination { get; set; }
    //    Transform DestinationIndicatorInWIM { get; set; }
    //    Transform DestinationIndicatorInLevel { get; set; }
    //    GameObject TravelPreviewAnimationObj { get; set; }
    //    Transform PlayerRepresentationTransform { get; set; }

    //    // should be local vars?
    //    GameObject TravelPreviewAnimationObj2 { get; set; }
    //    Transform LevelTransform { get; set; }
    //    Transform WIMLevelTransform { get; set; }
    //    Transform PlayerTransform { get; set; }
    //    Transform HMDTransform { get; set; }
    //    Transform FingertipIndexR { get; set; }
    //    Transform OVRPlayerController { get; set; }
    //    bool ArmLengthDetected { get; set; }
    //    PostTravelPathTrace PathTrace { get; set; }
    //    OVRGrabbable Grabbable { get; set; }
    //    float PrevInterHandDistance { get; set; }
    //    Transform HandL { get; set; }
    //    Transform HandR { get; set; }
    //    Hand ScalingHand { get; set; }
    //    Material PreviewScreenMaterial { get; set; }
    //    float WIMHeightRelativeToPlayer { get; set; }
    //    Vector3 WIMLevelLocalPosOnTravel { get; set; }
    //    bool isNewDestination { get; set; }
    //    Transform DestinationIndicatorInWIM2 { get; set; }
    //    bool PreviewScreenEnabled { get; set; }
    //    Vector3 WIMLevelLocalPos { get; set; }

    //    // Used for detecting changes via UI.
    //    bool PrevSemiTransparent { get; set; }
    //    float PrevTransparency { get; set; }
    //}

    //internal class WIMDataImpl : WIMData {
    //    public bool IsNewDestination { get; set; }
    //    public Transform DestinationIndicatorInWIM { get; set; }
    //    public Transform DestinationIndicatorInLevel { get; set; }
    //    public GameObject TravelPreviewAnimationObj { get; set; }
    //    public Transform PlayerRepresentationTransform { get; set; }
    //    public GameObject TravelPreviewAnimationObj2 { get; set; }
    //    public Transform LevelTransform { get; set; }
    //    public Transform WIMLevelTransform { get; set; }
    //    public Transform PlayerTransform { get; set; }
    //    public Transform HMDTransform { get; set; }
    //    public Transform FingertipIndexR { get; set; }
    //    public Transform OVRPlayerController { get; set; }
    //    public bool ArmLengthDetected { get; set; }
    //    public PostTravelPathTrace PathTrace { get; set; }
    //    public OVRGrabbable Grabbable { get; set; }
    //    public float PrevInterHandDistance { get; set; }
    //    public Transform HandL { get; set; }
    //    public Transform HandR { get; set; }
    //    public Hand ScalingHand { get; set; }
    //    public Material PreviewScreenMaterial { get; set; }
    //    public float WIMHeightRelativeToPlayer { get; set; }
    //    public Vector3 WIMLevelLocalPosOnTravel { get; set; }
    //    public bool isNewDestination { get; set; }
    //    public Transform DestinationIndicatorInWIM2 { get; set; }
    //    public bool PreviewScreenEnabled { get; set; }
    //    public Vector3 WIMLevelLocalPos { get; set; }
    //    public bool PrevSemiTransparent { get; set; }
    //    public float PrevTransparency { get; set; }
    //}


}