using UnityEngine;
using UnityEngine.Assertions;

namespace WIM_Plugin {
    [RequireComponent(typeof(LineRenderer))]
    [DisallowMultipleComponent]
    public class TravelPreviewAnimationController : MonoBehaviour {
        public WIMSpaceConverter Converter { get; set; }
        public Transform DestinationInWIM { get; set; }
        public Transform PlayerRepresentationInWIM { get; set; }
        public Transform WIMLevelTransform { get; set; }
        public GameObject DestinationIndicator { get; set; }
        public float AnimationSpeed { get; set; } = 1.0f;

        private LineRenderer lr;
        private Transform animatedPlayerRepresentation;
        private float animationProgress;
        private float startAnimationProgress;
        private float endAnimationProgress;


        private void Awake() {
            lr = GetComponent<LineRenderer>();
        }

        private void Start() {
            InitLineRenderer();
            InitAnimatedPlayerRepresentation();
            ResetAnimation();
        }

        private void InitLineRenderer() {
            lr.widthMultiplier = .001f;
            lr.material = Resources.Load<Material>("Materials/SemiTransparent");
        }

        private void InitAnimatedPlayerRepresentation() {
            Assert.IsNotNull(DestinationIndicator);
            Assert.IsNotNull(WIMLevelTransform);
            animatedPlayerRepresentation = GameObject.Instantiate(DestinationIndicator, WIMLevelTransform).transform;
            Assert.IsNotNull(animatedPlayerRepresentation);
            animatedPlayerRepresentation.gameObject.AddComponent<Destroyer>();
            animatedPlayerRepresentation.name = "Animated Player Travel Representation";
            animatedPlayerRepresentation.GetComponent<Renderer>().material =
                Resources.Load<Material>("Materials/SemiTransparent");
        }

        private void OnDestroy() {
            if(!animatedPlayerRepresentation) return;
            animatedPlayerRepresentation.gameObject.AddComponent<Destroyer>();
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