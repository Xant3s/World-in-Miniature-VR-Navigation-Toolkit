// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;
using UnityEngine.Assertions;
using WIMVR.Core;
using WIMVR.Util;

namespace WIMVR.Features.Travel_Preview_Animation {
    /// <summary>
    /// Plays the travel preview animation.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    [DisallowMultipleComponent]
    public class TravelPreviewAnimationController : MonoBehaviour {
        private LineRenderer lr;
        private Transform animatedPlayerRepresentation;
        private float animationProgress;
        private float startAnimationProgress;
        private float endAnimationProgress;

        /// <summary>
        /// The converter used to convert between world space and WIM space.
        /// </summary>
        public WIMSpaceConverter Converter { get; set; }

        /// <summary>
        /// The selected destination in the miniature model.
        /// </summary>
        public Transform DestinationInWIM { get; set; }

        /// <summary>
        /// The player's representation in the miniature model.
        /// This is the start position of the travel.
        /// </summary>
        public Transform PlayerRepresentationInWIM { get; set; }

        /// <summary>
        /// The miniature model level.
        /// </summary>
        public Transform WIMLevelTransform { get; set; }

        /// <summary>
        /// The destination indicator prefab.
        /// Used to visualize travel.
        /// </summary>
        public GameObject DestinationIndicator { get; set; }

        /// <summary>
        /// Specifies how fast the travel preview animation should be played.
        /// </summary>
        public float AnimationSpeed { get; set; } = 1.0f;


        private void Awake() {
            lr = GetComponent<LineRenderer>();
        }

        private void Start() {
            RegisterCleanupOnWIMRespawnEvent();
            InitLineRenderer();
            InitAnimatedPlayerRepresentation();
            ResetAnimation();
        }

        private void RegisterCleanupOnWIMRespawnEvent() {
            MiniatureModel.OnCleanupWIMBeforeRespawn += Cleanup;
        }

        private void InitLineRenderer() {
            lr.widthMultiplier = .001f;
            lr.material = Resources.Load<Material>("SemiTransparent");
        }

        private void InitAnimatedPlayerRepresentation() {
            Assert.IsNotNull(DestinationIndicator);
            Assert.IsNotNull(WIMLevelTransform);
            animatedPlayerRepresentation = Instantiate(DestinationIndicator, WIMLevelTransform).transform;
            animatedPlayerRepresentation.name = "Animated Player Travel Representation";
            animatedPlayerRepresentation.GetComponentInChildren<Renderer>().material =
                Resources.Load<Material>("SemiTransparent");
        }

        private void Cleanup(WIMConfiguration config, WIMData data) {
            if(!animatedPlayerRepresentation) return;
            animatedPlayerRepresentation.parent = null;    // Prevent from being copied on copy WIM.
            Destroy(animatedPlayerRepresentation.gameObject);
        }

        private void Update() {
            UpdateLineRenderer();
            UpdateAnimatedPlayerRepresentation();
        }

        private void UpdateLineRenderer() {
            lr.SetPosition(0, PlayerRepresentationInWIM.position);
            lr.SetPosition(1, DestinationInWIM.position);
        }

        private void UpdateAnimatedPlayerRepresentation() {
            if(animationProgress == 0) {
                // Start animation: Turn towards destination.
                DoStartAnimation();
            }
            else if(animationProgress >= 1) {
                // Reached destination. End animation: align with destination indicator.
                DoEndAnimation();
            }
            else {
                // In between: walk towards destination indicator.
                AdvanceWalkAnimation();
                UpdateRotation();
                UpdatePosition();
            }
        }

        private void DoStartAnimation() {
            var destinationInLevelSpace = Converter.ConvertToLevelSpace(DestinationInWIM.position);
            var playerPosInLevelSpace = Converter.ConvertToLevelSpace(animatedPlayerRepresentation.position);
            var rotationInLevelSpace =
                Quaternion.LookRotation(destinationInLevelSpace - playerPosInLevelSpace, Vector3.up);
            var desiredRotation = WIMLevelTransform.rotation * rotationInLevelSpace;
            if(startAnimationProgress >= 1) {
                animationProgress = .01f; // Start the next phase: move towards destination.
                return;
            }

            var step = AnimationSpeed * 4.0f * Time.deltaTime;
            startAnimationProgress += step;
            animatedPlayerRepresentation.rotation =
                Quaternion.Lerp(PlayerRepresentationInWIM.rotation, desiredRotation, startAnimationProgress);
            UpdatePosition();
        }

        private void DoEndAnimation() {
            var destinationInLevelSpace = Converter.ConvertToLevelSpace(DestinationInWIM.position);
            var playerPosInLevelSpace = Converter.ConvertToLevelSpace(PlayerRepresentationInWIM.position);
            var rotationInLevelSpace =
                Quaternion.LookRotation(destinationInLevelSpace - playerPosInLevelSpace, Vector3.up);
            var desiredRotation = WIMLevelTransform.rotation * rotationInLevelSpace;
            if(endAnimationProgress >= 1) {
                ResetAnimation();
                return;
            }

            var step = AnimationSpeed * 4.0f * Time.deltaTime;
            endAnimationProgress += step;
            animatedPlayerRepresentation.rotation =
                Quaternion.Lerp(desiredRotation, DestinationInWIM.rotation, endAnimationProgress);
            UpdatePosition();
        }

        private void UpdatePosition() {
            var dir = DestinationInWIM.position - PlayerRepresentationInWIM.position;
            animatedPlayerRepresentation.position = PlayerRepresentationInWIM.position + dir * animationProgress;
        }

        private void AdvanceWalkAnimation() {
            var step = AnimationSpeed * Time.deltaTime;
            animationProgress += step;
        }

        private void UpdateRotation() {
            var destinationInLevelSpace = Converter.ConvertToLevelSpace(DestinationInWIM.position);
            var playerPosInLevelSpace = Converter.ConvertToLevelSpace(animatedPlayerRepresentation.position);
            var rotationInLevelSpace =
                Quaternion.LookRotation(destinationInLevelSpace - playerPosInLevelSpace, Vector3.up);
            animatedPlayerRepresentation.rotation = WIMLevelTransform.rotation * rotationInLevelSpace;
        }

        private void ResetAnimation() {
            animatedPlayerRepresentation.position = PlayerRepresentationInWIM.position;
            animatedPlayerRepresentation.rotation = PlayerRepresentationInWIM.rotation;
            animationProgress = 0;
            startAnimationProgress = 0;
            endAnimationProgress = 0;
        }
    }
}