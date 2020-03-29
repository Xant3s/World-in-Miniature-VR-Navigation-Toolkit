using UnityEngine;
using UnityEngine.Assertions;

namespace WIM_Plugin {
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(OVRGrabbable))]
    [RequireComponent(typeof(DistanceGrabbable))]
    [RequireComponent(typeof(PlayerRepresentation))]
    [RequireComponent(typeof(Respawn))]
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class MiniatureModel : MonoBehaviour {
        public WIMConfiguration Configuration;
        public WIMData Data;
        public WIMSpaceConverter Converter;

        public delegate void WIMAction(WIMConfiguration config, WIMData data);
        public delegate void WIMAxisAction(WIMConfiguration config, WIMData data, float axis);
        public static event WIMAction OnInit;
        public static event WIMAction OnLateInit;
        public static event WIMAction OnUpdate;
        public static event WIMAction OnNewDestinationSelected;
        public static event WIMAction OnPreTravel;
        public static event WIMAction OnPostTravel;
        public static event WIMAction OnPickupIndexButtonDown;
        public static event WIMAction OnPickupIndexButtonUp;
        public static event WIMAxisAction OnPickpuIndexButton;
        public static event WIMAction OnPickupThumbButtonDown;
        public static event WIMAction OnPickupThumbButtonUp;
        public static event WIMAction OnPickupThumbTouchUp;
        public static event WIMAction OnLeftGrabButtonDown;
        public static event WIMAction OnLeftGrabButtonUp;
        public static event WIMAction OnRightGrabButtonDown;
        public static event WIMAction OnRightGrabButtonUp;

        private static readonly string pickupIndexActionName = "Pickup Index Button";
        private static readonly string pickupThumbActionName = "Pickup Thumb Button";
        private static readonly string grabLActionName = "Left Grab Button";
        private static readonly string grabRActionName = "Right Grab Button";
        private TravelStrategy travelStrategy;


        private void Awake() {
            if (!Application.isPlaying) return;
            if(!Configuration) return;
            Data = ScriptableObject.CreateInstance<WIMData>();
            travelStrategy = new InstantTravel();
            Data.LevelTransform = GameObject.FindWithTag("Level")?.transform;
            Data.PlayerTransform = GameObject.Find("OVRCameraRig")?.transform;
            Data.HMDTransform = GameObject.Find("CenterEyeAnchor")?.transform;
            Data.FingertipIndexR = GameObject.Find("hands:b_r_index_ignore")?.transform;
            Data.OVRPlayerController = GameObject.Find("OVRPlayerController")?.transform;
            Assert.IsNotNull(Data.HMDTransform);
            Assert.IsNotNull(Data.FingertipIndexR);
            Assert.IsNotNull(Configuration.PlayerRepresentation);
            Assert.IsNotNull(Configuration.DestinationIndicator);
            Assert.IsNotNull(Data.OVRPlayerController);
            Assert.IsNotNull(Data.LevelTransform);
            Assert.IsNotNull(Data.PlayerTransform);
        }

        private void Start() {
            if (!Application.isPlaying) return;
            Data.WIMLevelTransform = transform.Find("WIM Level");
            Assert.IsNotNull(Data.WIMLevelTransform);
            if(!Configuration) return;
            Converter = new WIMSpaceConverterImpl(Configuration, Data);
            OnInit?.Invoke(Configuration, Data);
            OnLateInit?.Invoke(Configuration, Data);
        }

        private void Update() {
            if (!Application.isPlaying) return;
            if(!Configuration) return;
            OnUpdate?.Invoke(Configuration, Data);
        }

        private void PickupThumbTouchUp() {
            if(!Configuration) return;
            OnPickupThumbTouchUp?.Invoke(Configuration, Data);
        }

        private void OnEnable() {
            var pickupIndexButtonTooltip = "Used to detect pickup when the destination selection method is set to pickup.";
            var pickupThumbButtonTooltip = "Used to detect pickup when the destination selection method is set to pickup.";
            var grabLTooltip = "The grab button on the left hand. Used to grab the miniature model.";
            var grabRTooltip = "The grab button on the right hand. Used to grab the miniature model.";
            InputManager.RegisterAction(pickupIndexActionName, PickupIndexButtonDown, InputManager.ButtonTrigger.ButtonDown, pickupIndexButtonTooltip);
            InputManager.RegisterAction(pickupIndexActionName, PickupIndexButtonUp, InputManager.ButtonTrigger.ButtonUp, pickupIndexButtonTooltip);
            InputManager.RegisterAction(pickupIndexActionName, PickupIndexButton);
            InputManager.RegisterAction(pickupThumbActionName, PickupThumbButtonDown, InputManager.ButtonTrigger.ButtonDown, pickupThumbButtonTooltip);
            InputManager.RegisterAction(pickupThumbActionName, PickupThumbButtonUp, InputManager.ButtonTrigger.ButtonUp, pickupThumbButtonTooltip);
            InputManager.RegisterTouchAction(pickupThumbActionName, PickupThumbTouchUp, InputManager.ButtonTrigger.ButtonUp, pickupThumbButtonTooltip);
            InputManager.RegisterAction(grabLActionName, LeftGrabButtonDown, InputManager.ButtonTrigger.ButtonDown, grabLTooltip);
            InputManager.RegisterAction(grabLActionName, LeftGrabButtonUp, InputManager.ButtonTrigger.ButtonUp, grabLTooltip);
            InputManager.RegisterAction(grabRActionName, RightGrabButtonDown, InputManager.ButtonTrigger.ButtonDown, grabRTooltip);
            InputManager.RegisterAction(grabRActionName, RightGrabButtonUp, InputManager.ButtonTrigger.ButtonUp, grabRTooltip);
        }

        private void OnDisable() {
            InputManager.UnregisterAction(pickupIndexActionName);
            InputManager.UnregisterAction(pickupThumbActionName);
            InputManager.UnregisterAction(grabLActionName);
            InputManager.UnregisterAction(grabRActionName);
        }

        private void PickupIndexButtonDown() {
            if(!Configuration) return;
            OnPickupIndexButtonDown?.Invoke(Configuration, Data);
        }

        private void PickupIndexButtonUp() {
            if(!Configuration) return;
            OnPickupIndexButtonUp?.Invoke(Configuration, Data);
        }

        private void PickupIndexButton(float axis) {
            if(!Configuration) return;
            OnPickpuIndexButton?.Invoke(Configuration, Data, axis);
        }

        private void PickupThumbButtonDown() {
            if(!Configuration) return;
            OnPickupThumbButtonDown?.Invoke(Configuration, Data);
        }

        private void PickupThumbButtonUp() {
            if(!Configuration) return;
            OnPickupThumbButtonUp?.Invoke(Configuration, Data);
        }

        private void LeftGrabButtonDown() {
            if(!Configuration) return;
            OnLeftGrabButtonDown?.Invoke(Configuration, Data);
        }

        private void LeftGrabButtonUp() {
            if(!Configuration) return;
            OnLeftGrabButtonUp?.Invoke(Configuration, Data);
        }

        private void RightGrabButtonDown() {
            if(!Configuration) return;
            OnRightGrabButtonDown?.Invoke(Configuration, Data);
        }

        private void RightGrabButtonUp() {
            if(!Configuration) return;
            OnRightGrabButtonUp?.Invoke(Configuration, Data);
        }

        public void NewDestination() {
            if(!Configuration) return;
            OnNewDestinationSelected?.Invoke(Configuration, Data);
        }

        public void ConfirmTravel() {
            if(!Configuration) return;
            DestinationIndicators.RemoveDestinationIndicators(this);
            OnPreTravel?.Invoke(Configuration, Data);
            travelStrategy.Travel(this);
            OnPostTravel?.Invoke(Configuration, Data);
        }
    }
}