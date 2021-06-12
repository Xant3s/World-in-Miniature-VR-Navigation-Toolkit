// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;
using UnityEngine.Assertions;
using WIMVR.Core.Input;
using WIMVR.Features;
using WIMVR.Features.LiveUpdate.Tags;
using WIMVR.Util;
using WIMVR.VR;

namespace WIMVR.Core {
    /// <summary>
    /// The core miniature model component. Turns this gameobject into a miniature model.
    /// Add additional feature components to modify functionality.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(OffsetGrabInteractable))]
    [RequireComponent(typeof(PlayerRepresentation))]
    [RequireComponent(typeof(Respawn))]
    [RequireComponent(typeof(WIMInput))]
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class MiniatureModel : MonoBehaviour {
        public WIMConfiguration Configuration;
        public WIMData Data;
        public WIMSpaceConverter Converter;
        
        #region Events
        public delegate void WIMAction(WIMConfiguration config, WIMData data);
       
        public static event WIMAction OnInit;
        public static event WIMAction OnInitHand;
        public static event WIMAction OnLateInitHand;
        public static event WIMAction OnLateInit;
        public static event WIMAction OnUpdate;
        public static event WIMAction OnNewDestinationSelected;
        public static event WIMAction OnPreTravel;
        public static event WIMAction OnPostTravel;
        public static event WIMAction OnCleanupWIMBeforeRespawn;
        #endregion

        private TravelStrategy travelStrategy;
        private int numberOfHandsSpawned;
        private OffsetGrabInteractable grabbable;
        private Rigidbody _rigidbody;


        public void NewDestination() {
            if(!Configuration) return;
            OnNewDestinationSelected?.Invoke(Configuration, Data);
        }

        /// <summary>
        /// Starts the travel phase.
        /// </summary>
        public void ConfirmTravel() {
            if(!Configuration) return;
            DestinationIndicators.RemoveDestinationIndicators(Configuration, Data);
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
            Data.LevelTransform = FindObjectOfType<Level>()?.transform;
            Data.PlayerTransform = GameObject.FindWithTag("Player")?.transform;
            Data.HMDTransform = GameObject.FindWithTag("MainCamera")?.transform;
            Data.PlayerController = GameObject.FindWithTag("Player")?.transform;
            Data.WIMLevelTransform = transform.Find("WIM Level");
            grabbable = GetComponent<OffsetGrabInteractable>();
            _rigidbody = GetComponent<Rigidbody>();
            Assert.IsNotNull(Data.HMDTransform);
            Assert.IsNotNull(Configuration.PlayerRepresentation);
            Assert.IsNotNull(Configuration.DestinationIndicator);
            Assert.IsNotNull(Data.PlayerController);
            Assert.IsNotNull(Data.LevelTransform);
            Assert.IsNotNull(Data.PlayerTransform);
            Assert.IsNotNull(Data.WIMLevelTransform);
            Converter = new WIMSpaceConverterImpl(Configuration, Data);
            OnInit?.Invoke(Configuration, Data);
            OnLateInit?.Invoke(Configuration, Data);
        }

        public void HandSpawned() {
            if(++numberOfHandsSpawned > 1) {
                OnInitHand?.Invoke(Configuration, Data);
                OnLateInitHand?.Invoke(Configuration, Data);
            }
        }

        public void CleanupBeforeRespawn() {
            OnCleanupWIMBeforeRespawn?.Invoke(Configuration, Data);
        }

        private void Update() {
            if (!Application.isPlaying) return;
            if(!Configuration) return;
            OnUpdate?.Invoke(Configuration, Data);
            if(!grabbable.IsGrabbed) ResetRigidbody();
        }

        private void ResetRigidbody() {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }

        private void OnEnable() {
            if(!Application.isPlaying) {
                gameObject.layer = LayerMask.NameToLayer("WIM");
                GetComponent<Rigidbody>().useGravity = false;
                if(name.Equals("GameObject")) name = "Miniature Model";
            }
        }
    }
}