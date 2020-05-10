// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using WIMVR.Features;
using WIMVR.Util;
using WIMVR.Input;

namespace WIMVR.Core {
    /// <summary>
    /// The core miniature model component. Turns this gameobject into a miniature model.
    /// Add additional feature components to modify functionality.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(XRGrabInteractable))]
    [RequireComponent(typeof(PlayerRepresentation))]
    [RequireComponent(typeof(Respawn))]
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class MiniatureModel : MonoBehaviour {
        #region Input Mappings Tooltips
        private static readonly string pickupIndexActionName = "Pickup Index Button";
        private static readonly string pickupThumbActionName = "Pickup Thumb Button";
        private static readonly string pickupThumbActionName2 = "Pickup Thumb Button (touch)";
        private static readonly string grabLActionName = "Left Grab Button";
        private static readonly string grabRActionName = "Right Grab Button";
        private static readonly string pickupIndexButtonTooltip = "Used to detect pickup when the destination selection method is set to pickup.";
        private static readonly string pickupThumbButtonTooltip = "Used to detect pickup when the destination selection method is set to pickup.";
        private static readonly string pickupThumbButtonTooltip2 = "This should have the same key assigned as 'pickup thumb button'. Allows the player to pick something up by just touching the thumb button instead of pressing it.";
        private static readonly string grabLTooltip = "The grab button on the left hand. Used to grab the miniature model.";
        private static readonly string grabRTooltip = "The grab button on the right hand. Used to grab the miniature model.";
        #endregion

        #region Events
        public delegate void WIMAction(WIMConfiguration config, WIMData data);

        public delegate void WIMAxisAction(WIMConfiguration config, WIMData data, float axis);

        public WIMConfiguration Configuration;
        public WIMData Data;
        public WIMSpaceConverter Converter;
        private TravelStrategy travelStrategy;
        public static event WIMAction OnInit;
        public static event WIMAction OnInitHand;
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
        #endregion

        private int numberOfHandsSpawned;



        public void NewDestination() {
            if(!Configuration) return;
            OnNewDestinationSelected?.Invoke(Configuration, Data);
        }

        /// <summary>
        /// Starts the travel phase.
        /// </summary>
        public void ConfirmTravel() {
            if(!Configuration) return;
            DestinationIndicators.RemoveDestinationIndicators(this);
            OnPreTravel?.Invoke(Configuration, Data);
            travelStrategy.Travel(this);
            OnPostTravel?.Invoke(Configuration, Data);
        }

        private void Start() {
            if (!Application.isPlaying) return;
            if (!Configuration) return;
            Data = ScriptableObject.CreateInstance<WIMData>();
            Assert.IsNotNull(Data);
            travelStrategy = new InstantTravel();
            Data.LevelTransform = GameObject.FindWithTag("Level")?.transform;
            Data.PlayerTransform = GameObject.FindWithTag("Player")?.transform;
            Data.HMDTransform = GameObject.FindWithTag("MainCamera")?.transform;
            Data.FingertipIndexR = GameObject.Find("hands:b_r_index_ignore")?.transform;
            Data.OVRPlayerController = GameObject.FindWithTag("Player")?.transform;
            Data.WIMLevelTransform = transform.Find("WIM Level");
            Assert.IsNotNull(Data.HMDTransform);
            //Assert.IsNotNull(Data.FingertipIndexR);
            Assert.IsNotNull(Configuration.PlayerRepresentation);
            Assert.IsNotNull(Configuration.DestinationIndicator);
            Assert.IsNotNull(Data.OVRPlayerController);
            Assert.IsNotNull(Data.LevelTransform);
            Assert.IsNotNull(Data.PlayerTransform);
            Assert.IsNotNull(Data.WIMLevelTransform);
            Converter = new WIMSpaceConverterImpl(Configuration, Data);
            OnInit?.Invoke(Configuration, Data);
            //OnLateInit?.Invoke(Configuration, Data);
        }

        public void ReInitWIM() {
            Debug.Log("ReInit WIM");
            Data.FingertipIndexR = GameObject.Find("hands:b_r_index_ignore")?.transform;
            Assert.IsNotNull(Data.FingertipIndexR);
        }

        public void HandSpawned() {
            if(++numberOfHandsSpawned > 1) {
                OnInitHand?.Invoke(Configuration, Data);
            }
        }

        private void Update() {
            if (!Application.isPlaying) return;
            if(!Configuration) return;
            OnUpdate?.Invoke(Configuration, Data);
        }

        private void PickupThumbTouchUp() {
            if(!Configuration) return;
            //OnPickupThumbTouchUp?.Invoke(Configuration, Data);
        }

        //public void OnTest(InputValue value) {
        //    Debug.Log("Test");
        //    Debug.Log(value.Get<Vector2>());
        //}

        private void OnEnable() {
            //InputManager.RegisterAction(pickupIndexActionName, PickupIndexButtonDown, InputManager.ButtonTrigger.ButtonDown, pickupIndexButtonTooltip);
            //InputManager.RegisterAction(pickupIndexActionName, PickupIndexButtonUp, InputManager.ButtonTrigger.ButtonUp, pickupIndexButtonTooltip);
            //InputManager.RegisterAction(pickupIndexActionName, PickupIndexButton);
            //InputManager.RegisterAction(pickupThumbActionName, PickupThumbButtonDown, InputManager.ButtonTrigger.ButtonDown, pickupThumbButtonTooltip);
            //InputManager.RegisterAction(pickupThumbActionName, PickupThumbButtonUp, InputManager.ButtonTrigger.ButtonUp, pickupThumbButtonTooltip);
            //InputManager.RegisterTouchAction(pickupThumbActionName2, PickupThumbTouchUp, InputManager.ButtonTrigger.ButtonUp, pickupThumbButtonTooltip2);
            //InputManager.RegisterAction(grabLActionName, LeftGrabButtonDown, InputManager.ButtonTrigger.ButtonDown, grabLTooltip);
            //InputManager.RegisterAction(grabLActionName, LeftGrabButtonUp, InputManager.ButtonTrigger.ButtonUp, grabLTooltip);
            //InputManager.RegisterAction(grabRActionName, RightGrabButtonDown, InputManager.ButtonTrigger.ButtonDown, grabRTooltip);
            //InputManager.RegisterAction(grabRActionName, RightGrabButtonUp, InputManager.ButtonTrigger.ButtonUp, grabRTooltip);
            if(!Application.isPlaying) {
                gameObject.tag = "WIM";
                gameObject.layer = LayerMask.NameToLayer("WIM");
                GetComponent<Rigidbody>().useGravity = false;
                if(name.Equals("GameObject")) name = "Miniature Model";
            }
        }

        private void OnDisable() {
            //InputManager.UnregisterAction(pickupIndexActionName);
            //InputManager.UnregisterAction(pickupThumbActionName);
            //InputManager.UnregisterAction(pickupThumbActionName2);
            //InputManager.UnregisterAction(grabLActionName);
            //InputManager.UnregisterAction(grabRActionName);
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
    }
}