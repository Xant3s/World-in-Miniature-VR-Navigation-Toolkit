using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace WIM_Plugin {
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(OVRGrabbable))]
    [RequireComponent(typeof(DistanceGrabbable))]
    [ExecuteAlways]
    public class MiniatureModel : MonoBehaviour {
        public WIMConfiguration Configuration;
        public WIMData Data;
        public WIMSpaceConverter Converter;

        public delegate void WIMAction(WIMConfiguration config, WIMData data);

        public static event WIMAction OnInit;
        public static event WIMAction OnLateInit;
        public static event WIMAction OnUpdate;
        public static event WIMAction OnNewDestinationSelected;
        public static event WIMAction OnPreTravel;
        public static event WIMAction OnPostTravel;
        //public static event WIMAction OnPickupIndexButton;
        //public static event WIMAction OnPickupThumbButton;
        public static event WIMAction OnPickupIndexButtonDown;
        public static event WIMAction OnPickupIndexButtonUp;
        public static event WIMAction OnPickupThumbButtonDown;
        public static event WIMAction OnPickupThumbButtonUp;


        private static readonly string pickupIndexActionName = "Pickup Index Button";
        private static readonly string pickupThumbActionName = "Pickup Thumb Button";
        private TravelStrategy travelStrategy;


        private void Awake() {
            if (!Application.isPlaying) return;
            if (!ConfigurationIsThere()) return;
            Data = ScriptableObject.CreateInstance<WIMData>();
            Converter = new WIMSpaceConverterImpl(Configuration, Data);
            travelStrategy = new InstantTravel();
            Data.WIMLevelTransform = GameObject.Find("WIM Level")?.transform;
            Data.LevelTransform = GameObject.Find("Level")?.transform;
            Data.PlayerTransform = GameObject.Find("OVRCameraRig")?.transform;
            Data.HMDTransform = GameObject.Find("CenterEyeAnchor")?.transform;
            Data.FingertipIndexR = GameObject.Find("hands:b_r_index_ignore")?.transform;
            Data.OVRPlayerController = GameObject.Find("OVRPlayerController")?.transform;
            Assert.IsNotNull(Data.WIMLevelTransform);
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
            if (!ConfigurationIsThere()) return;
            OnInit?.Invoke(Configuration, Data);
            OnLateInit?.Invoke(Configuration, Data);
        }

        private void Update() {
            if (!Application.isPlaying) return;
            if (!ConfigurationIsThere()) return;
            OnUpdate?.Invoke(Configuration, Data);
        }

        private void OnEnable() {
            //InputManager.RegisterAction(pickupIndexActionName, pickupIndexButtonEvent, InputManager.ButtonTrigger.ButtonUpAndDown);
            //InputManager.RegisterAction(pickupThumbActionName, pickupThumbButtonEvent, InputManager.ButtonTrigger.ButtonUpAndDown);

            InputManager.RegisterAction(pickupIndexActionName, pickupIndexButtonDown, InputManager.ButtonTrigger.ButtonDown);
            InputManager.RegisterAction(pickupIndexActionName, pickpuIndexButtonUp, InputManager.ButtonTrigger.ButtonUp);
            InputManager.RegisterAction(pickupThumbActionName, pickupThumbButtonDown, InputManager.ButtonTrigger.ButtonDown);
            InputManager.RegisterAction(pickupThumbActionName, pickupThumbButtonUp, InputManager.ButtonTrigger.ButtonUp);
        }

        private void OnDisable() {
            InputManager.UnregisterAction(pickupIndexActionName);
            InputManager.UnregisterAction(pickupThumbActionName);
        }

        //private void pickupIndexButtonEvent() {
        //    OnPickupIndexButton?.Invoke(Configuration, Data);
        //}

        //private void pickupThumbButtonEvent() {
        //    OnPickupThumbButton?.Invoke(Configuration, Data);
        //}

        private void pickupIndexButtonDown() {
            OnPickupIndexButtonDown?.Invoke(Configuration, Data);
        }

        private void pickpuIndexButtonUp() {
            OnPickupIndexButtonUp?.Invoke(Configuration, Data);

        }

        private void pickupThumbButtonDown() {
            OnPickupThumbButtonDown?.Invoke(Configuration, Data);

        }

        private void pickupThumbButtonUp() {
            OnPickupThumbButtonUp?.Invoke(Configuration, Data);

        }

        private bool ConfigurationIsThere() {
            if(Configuration) return true;
            throw new Exception("WIM configuration missing.");
        }

        public void NewDestination() {
            OnNewDestinationSelected?.Invoke(Configuration, Data);
        }

        public void ConfirmTravel() {
            DestinationIndicators.RemoveDestinationIndicators(this);
            OnPreTravel?.Invoke(Configuration, Data);
            travelStrategy.Travel(this);
            OnPostTravel?.Invoke(Configuration, Data);
        }
    }
}