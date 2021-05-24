// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using WIMVR.Core;
using WIMVR.Core.Input;
using WIMVR.Util;
using WIMVR.Util.XR;
using Hand = WIMVR.Util.Hand;

namespace WIMVR.Features.Scaling {
    /// <summary>
    /// Allows to scale the miniature model at runtime.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(OffsetGrabInteractable))]
    public class Scaling : MonoBehaviour {
        public ScalingConfiguration ScalingConfig;
        private WIMConfiguration config;
        private IHandInitializer<InputDevice> handInitializer;
        private IButtonListener rightGrabButtonListener;
        private IButtonListener leftGrabButtonListener;
        private OffsetGrabInteractable grabbable;
        private Transform handR;
        private Transform handL;
        private Transform WIMTransform;
        private SphereCollider rightGrabVolume;
        private SphereCollider leftGrabVolume;
        private Hand scalingHand = Hand.None;
        private float prevInterHandDistance;

        
        private void OnEnable() {
            if(!ScalingConfig) return;
            MiniatureModel.OnUpdate += ScaleWIM;
        }

        private void OnDisable() {
            if(!ScalingConfig) return;
            MiniatureModel.OnUpdate -= ScaleWIM;
        }

        private void Awake() {
            if(!ScalingConfig) return;
            WIMTransform = GameObject.FindWithTag("WIM").transform;
            grabbable = WIMTransform.GetComponent<OffsetGrabInteractable>();
        }

        private void Start() {
            Init();
            handInitializer = new XRControllerInitializer();
            handInitializer.OnRightHandInitialized += RightHandInitialized;
            handInitializer.OnLeftHandInitialized += LeftHandInitialized;
            handInitializer.StartWaitForHands();
        }

        private void Update() {
            if (!IsInitialized()) Init();
            
            rightGrabButtonListener?.Update();
            leftGrabButtonListener?.Update();
        }

        private void RightHandInitialized(InputDevice rightController) {
            var grabButton = XRUtils.DetectGrabButton(Hand.RightHand);
            rightGrabButtonListener = new ButtonListener(grabButton, rightController);
            rightGrabButtonListener.OnButtonDown += () => SetScalingHand(Hand.RightHand);
            rightGrabButtonListener.OnButtonUp += () => scalingHand = Hand.None;
        }

        private void LeftHandInitialized(InputDevice leftController) {
            var grabButton = XRUtils.DetectGrabButton(Hand.LeftHand);
            leftGrabButtonListener = new ButtonListener(grabButton, leftController);
            leftGrabButtonListener.OnButtonDown += () => SetScalingHand(Hand.LeftHand);
            leftGrabButtonListener.OnButtonUp += () => scalingHand = Hand.None;
        }

        private bool IsInitialized() => handL && handR && leftGrabVolume && rightGrabVolume;

        private void Init() {
            handR = GameObject.FindWithTag("HandR")?.transform;
            handL = GameObject.FindWithTag("HandL")?.transform;

            if(handR) rightGrabVolume = handR.GetComponentInParent<SphereCollider>();
            if(handL) leftGrabVolume = handL.GetComponentInParent<SphereCollider>();
        }

        private void SetScalingHand(Hand hand) {
            // Only if WIM scaling is enabled and WIM is currently being grabbed with one hand.
            if (!ScalingConfig.AllowWIMScaling || !grabbable.IsGrabbed) return;

            if (!IsInitialized()) Init();

            var grabbingHand = GetGrabbingHand();
            var oppositeHand = TypeUtils.GetOppositeHand(grabbingHand); // This is the potential scaling hand.
            if (oppositeHand != hand) return;

            // Start scaling if the potential scaling hand (the hand currently not grabbing the WIM) is inside the WIM and starts grabbing.
            if (GetHandIsInside(oppositeHand)) {
                scalingHand = oppositeHand;
            }
        }

        private void ScaleWIM(WIMConfiguration configuration, WIMData data) {
            config = configuration;
            Assert.IsNotNull(ScalingConfig, "Scaling configuration is missing.");

            // Only if WIM scaling is enabled and WIM is currently being grabbed with one hand.
            if (!ScalingConfig.AllowWIMScaling || !grabbable.IsGrabbed) return;

            // Check if currently scaling. Abort if not.
            if (scalingHand == Hand.None) return;

            // Scale using inter hand distance delta.
            var currInterHandDistance = Vector3.Distance(handL.position, handR.position);
            var distanceDelta = currInterHandDistance - prevInterHandDistance;
            var deltaBeyondThreshold = Mathf.Abs(distanceDelta) >= ScalingConfig.InterHandDistanceDeltaThreshold;
            if (distanceDelta > 0 && deltaBeyondThreshold) {
                config.ScaleFactor += ScalingConfig.ScaleStep;
            }
            else if (distanceDelta < 0 && deltaBeyondThreshold) {
                config.ScaleFactor -= ScalingConfig.ScaleStep;
            }
            config.ScaleFactor = Mathf.Clamp(config.ScaleFactor, ScalingConfig.MinScaleFactor, ScalingConfig.MaxScaleFactor);

            // Apply scale factor.
            WIMTransform.localScale = new Vector3(config.ScaleFactor, config.ScaleFactor, config.ScaleFactor);

            prevInterHandDistance = currInterHandDistance;
        }

        private Hand GetGrabbingHand() {
            if (!grabbable || !grabbable.selectingInteractor) return Hand.None;
            var xrController = grabbable?.selectingInteractor?.GetComponent<XRController>();
            if (xrController == null) return Hand.None;
            return xrController.controllerNode == XRNode.LeftHand ? Hand.LeftHand : Hand.RightHand; 
        }

        private bool GetHandIsInside(Hand hand) {
            if (hand == Hand.None) return false;
            var grabVolume = hand == Hand.LeftHand ? leftGrabVolume : rightGrabVolume;
            GetWorldSpaceSphere(grabVolume, out var center, out var radius);
            var hitColliders = Physics.OverlapSphere(center, radius, LayerMask.GetMask("WIM"));
            return hitColliders.Length != 0;
        }

        private static void GetWorldSpaceSphere(SphereCollider sphere, out Vector3 center, out float radius) {
            center = sphere.transform.TransformPoint(sphere.center);
            radius = sphere.radius;
        }
    }
}