using UnityEngine;
using UnityEngine.Assertions;
using WIM_Plugin;

namespace WIM_Plugin {
    public class TravelPreviewAnimation : MonoBehaviour {
        void OnEnable() {
            MiniatureModel.OnNewDestinationSelected += createController;
        }

        void OnDisable() {
            MiniatureModel.OnNewDestinationSelected -= createController;
        }

        private void createController(WIMConfiguration config, WIMData data) {
            if(!config.TravelPreviewAnimation) return;
            data.TravelPreviewAnimationObj.transform.parent = null;
            Destroy(data.TravelPreviewAnimationObj);
            data.TravelPreviewAnimationObj = new GameObject("Travel Preview Animation");
            var travelPreview = data.TravelPreviewAnimationObj.AddComponent<TravelPreviewAnimationController>();
            travelPreview.DestinationInWIM = data.DestinationIndicatorInWIM;
            travelPreview.PlayerRepresentationInWIM = data.PlayerRepresentationTransform;
            travelPreview.DestinationIndicator = config.DestinationIndicator;
            travelPreview.AnimationSpeed = config.TravelPreviewAnimationSpeed;
            travelPreview.WIMLevelTransform = data.WIMLevelTransform;
            travelPreview.Converter = GameObject.Find("WIM").GetComponent<MiniatureModel>().Converter;
        }
    }

    [RequireComponent(typeof(LineRenderer))]
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

        void Start() {
            initLineRenderer();
            initAnimatedPlayerRepresentation();
            resetAnimation();
        }

        private void initLineRenderer() {
            lr.widthMultiplier = .001f;
            lr.material = Resources.Load<Material>("Materials/SemiTransparent");
        }

        private void initAnimatedPlayerRepresentation() {
            Assert.IsNotNull(DestinationIndicator);
            Assert.IsNotNull(WIMLevelTransform);
            animatedPlayerRepresentation = GameObject.Instantiate(DestinationIndicator, WIMLevelTransform).transform;
            Assert.IsNotNull(animatedPlayerRepresentation);
            animatedPlayerRepresentation.gameObject.AddComponent<Destroyer>();
            animatedPlayerRepresentation.name = "Animated Player Travel Representation";
            animatedPlayerRepresentation.GetComponent<Renderer>().material =
                Resources.Load<Material>("Materials/SemiTransparent");
        }

        void OnDestroy() {
            if(!animatedPlayerRepresentation) return;
            animatedPlayerRepresentation.gameObject.AddComponent<Destroyer>();
            Destroy(animatedPlayerRepresentation.gameObject);
        }

        void Update() {
            updateLineRenderer();
            updateAnimatedPlayerRepresentation();
        }

        private void updateLineRenderer() {
            lr.SetPosition(0, PlayerRepresentationInWIM.position);
            lr.SetPosition(1, DestinationInWIM.position);
        }

        private void updateAnimatedPlayerRepresentation() {
            if(animationProgress == 0) {
                // Start animation: Turn towards destination.
                doStartAnimation();
            }
            else if(animationProgress >= 1) {
                // Reached destination. End animation: align with destination indicator.
                doEndAnimation();
            }
            else {
                // In between: walk towards destination indicator.
                advanceWalkAnimation();
                updateRotation();
                updatePosition();
            }
        }

        private void doStartAnimation() {
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
            updatePosition();
        }

        private void doEndAnimation() {
            var destinationInLevelSpace = Converter.ConvertToLevelSpace(DestinationInWIM.position);
            var playerPosInLevelSpace = Converter.ConvertToLevelSpace(PlayerRepresentationInWIM.position);
            var rotationInLevelSpace =
                Quaternion.LookRotation(destinationInLevelSpace - playerPosInLevelSpace, Vector3.up);
            var desiredRotation = WIMLevelTransform.rotation * rotationInLevelSpace;
            if(endAnimationProgress >= 1) {
                resetAnimation();
                return;
            }

            var step = AnimationSpeed * 4.0f * Time.deltaTime;
            endAnimationProgress += step;
            animatedPlayerRepresentation.rotation =
                Quaternion.Lerp(desiredRotation, DestinationInWIM.rotation, endAnimationProgress);
            updatePosition();
        }

        private void updatePosition() {
            var dir = DestinationInWIM.position - PlayerRepresentationInWIM.position;
            animatedPlayerRepresentation.position = PlayerRepresentationInWIM.position + dir * animationProgress;
        }

        private void advanceWalkAnimation() {
            var step = AnimationSpeed * Time.deltaTime;
            animationProgress += step;
        }

        private void updateRotation() {
            var destinationInLevelSpace = Converter.ConvertToLevelSpace(DestinationInWIM.position);
            var playerPosInLevelSpace = Converter.ConvertToLevelSpace(animatedPlayerRepresentation.position);
            var rotationInLevelSpace =
                Quaternion.LookRotation(destinationInLevelSpace - playerPosInLevelSpace, Vector3.up);
            animatedPlayerRepresentation.rotation = WIMLevelTransform.rotation * rotationInLevelSpace;
        }

        private void resetAnimation() {
            animatedPlayerRepresentation.position = PlayerRepresentationInWIM.position;
            animatedPlayerRepresentation.rotation = PlayerRepresentationInWIM.rotation;
            animationProgress = 0;
            startAnimationProgress = 0;
            endAnimationProgress = 0;
        }
    }
}