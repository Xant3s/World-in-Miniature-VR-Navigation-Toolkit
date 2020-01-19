using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace WIM_Plugin {
    // Allow scaling the WIM at runtime.
    [ExecuteAlways]
    public class Scaling : MonoBehaviour {
        public ScalingConfiguration ScalingConfig;
        private WIMConfiguration config;
        private WIMData data;
        private OVRGrabbable grabbable;
        private Transform handL;
        private Transform handR;
        private Transform WIMTransform;
        private CapsuleCollider leftGrabVolume;
        private CapsuleCollider rightGrabVolume;
        private Hand scalingHand = Hand.None;
        private float prevInterHandDistance;


        private void OnEnable() {
            if(!ScalingConfig) return;
            MiniatureModel.OnUpdate += ScaleWIM;
            MiniatureModel.OnLeftGrabButtonDown += leftScalingButtonDown;
            MiniatureModel.OnLeftGrabButtonUp += leftScalingButtonUp;
            MiniatureModel.OnRightGrabButtonDown += rightScalingButtonDown;
            MiniatureModel.OnRightGrabButtonUp += rightScalingButtonUp;
        }

        private void OnDisable() {
            if(!ScalingConfig) return;
            MiniatureModel.OnUpdate -= ScaleWIM;
            MiniatureModel.OnLeftGrabButtonDown -= leftScalingButtonDown;
            MiniatureModel.OnLeftGrabButtonUp -= leftScalingButtonUp;
            MiniatureModel.OnRightGrabButtonDown -= rightScalingButtonDown;
            MiniatureModel.OnRightGrabButtonUp -= rightScalingButtonUp;
        }

        private void Awake() {
            if(!ScalingConfig) return;
            WIMTransform = GameObject.FindWithTag("WIM").transform;
            grabbable = WIMTransform.GetComponent<OVRGrabbable>();
            handL = GameObject.FindWithTag("HandL").transform;
            handR = GameObject.FindWithTag("HandR").transform;
            leftGrabVolume = handL.GetComponentInChildren<CapsuleCollider>();
            rightGrabVolume = handR.GetComponentInChildren<CapsuleCollider>();
            Assert.IsNotNull(leftGrabVolume);
            Assert.IsNotNull(rightGrabVolume);
            Assert.IsNotNull(grabbable);
            Assert.IsNotNull(handL);
            Assert.IsNotNull(handR);
        }

        private void leftScalingButtonDown(WIMConfiguration config, WIMData data) {
            setScalingHand(Hand.LeftHand);
        }

        private void leftScalingButtonUp(WIMConfiguration config, WIMData data) {
            scalingHand = Hand.None;
        }

        private void rightScalingButtonDown(WIMConfiguration config, WIMData data) {
            setScalingHand(Hand.RightHand);
        }

        private void rightScalingButtonUp(WIMConfiguration config, WIMData data) {
            scalingHand = Hand.None;
        }

        private void setScalingHand(Hand hand) {
            // Only if WIM scaling is enabled and WIM is currently being grabbed with one hand.
            if (!ScalingConfig.AllowWIMScaling || !grabbable.isGrabbed) return;

            var grabbingHand = getGrabbingHand();
            var oppositeHand = getOppositeHand(grabbingHand); // This is the potential scaling hand.
            if (oppositeHand != hand) return;

            // Start scaling if the potential scaling hand (the hand currently not grabbing the WIM) is inside the WIM and starts grabbing.
            if (getHandIsInside(oppositeHand)) {
                scalingHand = oppositeHand;
            }
        }

        private void ScaleWIM(WIMConfiguration configuration, WIMData data) {
            this.config = configuration;
            this.data = data;
            Assert.IsNotNull(ScalingConfig, "Scaling configuration is missing.");

            // Only if WIM scaling is enabled and WIM is currently being grabbed with one hand.
            if (!ScalingConfig.AllowWIMScaling || !grabbable.isGrabbed) return;

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

        private Hand getGrabbingHand() {
            return grabbable.grabbedBy.CompareTag("HandL") ? Hand.LeftHand : Hand.RightHand;
        }

        private Hand getOppositeHand(Hand hand) {
            if (hand == Hand.None) return Hand.None;
            return (hand == Hand.LeftHand) ? Hand.RightHand : Hand.LeftHand;
        }

        private bool getHandIsInside(Hand hand) {
            if (hand == Hand.None) return false;
            var grabVolume = hand == Hand.LeftHand ? leftGrabVolume : rightGrabVolume;
            GetWorldSpaceCapsule(grabVolume, out var p1, out var p2, out var radius);
            var hitColliders = Physics.OverlapCapsule(p1, p2, radius, LayerMask.GetMask("WIM"));
            return hitColliders.Length != 0;
        }

        private static void GetWorldSpaceCapsule(CapsuleCollider capsule, out Vector3 point0, out Vector3 point1, out float radius) {
            var center = capsule.transform.TransformPoint(capsule.center);
            radius = 0f;
            var height = 0f;
            var lossyScale = capsule.transform.lossyScale;
            lossyScale = new Vector3(Mathf.Abs(lossyScale.x), Mathf.Abs(lossyScale.y), Mathf.Abs(lossyScale.z));
            var dir = Vector3.zero;

            switch(capsule.direction) {
                case 0: // x
                    radius = Mathf.Max(lossyScale.y, lossyScale.z) * capsule.radius;
                    height = lossyScale.x * capsule.height;
                    dir = capsule.transform.TransformDirection(Vector3.right);
                    break;
                case 1: // y
                    radius = Mathf.Max(lossyScale.x, lossyScale.z) * capsule.radius;
                    height = lossyScale.y * capsule.height;
                    dir = capsule.transform.TransformDirection(Vector3.up);
                    break;
                case 2: // z
                    radius = Mathf.Max(lossyScale.x, lossyScale.y) * capsule.radius;
                    height = lossyScale.z * capsule.height;
                    dir = capsule.transform.TransformDirection(Vector3.forward);
                    break;
            }

            if(height < radius * 2f) {
                dir = Vector3.zero;
            }

            point0 = center + dir * (height * 0.5f - radius);
            point1 = center - dir * (height * 0.5f - radius);
        }
    }
}