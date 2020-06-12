// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


namespace WIMVR.Core {
    public class OffsetGrabInteractable : XRGrabInteractable {
        [SerializeField] private bool restoreRigidbodyOnRelease;

        private Vector3 interactorPosition;
        private Quaternion interactorRotation;
        private float drag;
        private float angularDrag;


        protected override void OnSelectEnter(XRBaseInteractor interactor) {
            SaveRigidbody();
            base.OnSelectEnter(interactor);
            interactorPosition = interactor.attachTransform.localPosition;
            interactorRotation = interactor.attachTransform.localRotation;
            interactor.attachTransform.position = attachTransform ? attachTransform.position : transform.position;
            interactor.attachTransform.rotation = attachTransform ? attachTransform.rotation : transform.rotation;
        }

        protected override void OnSelectExit(XRBaseInteractor interactor) {
            base.OnSelectExit(interactor);
            interactor.attachTransform.localPosition = interactorPosition;
            interactor.attachTransform.localRotation = interactorRotation;
            interactorPosition = Vector3.zero;
            interactorRotation = Quaternion.identity;
            RestoreRigidbody();
        }

        private void SaveRigidbody() {
            var rb = GetComponent<Rigidbody>();
            if(restoreRigidbodyOnRelease && rb) {
                drag = rb.drag;
                angularDrag = rb.angularDrag;
            }
        }

        private void RestoreRigidbody() {
            var rb = GetComponent<Rigidbody>();
            if(rb && restoreRigidbodyOnRelease) {
                rb.angularVelocity = Vector3.zero;
                rb.velocity = Vector3.zero;
                rb.drag = drag;
                rb.angularDrag = angularDrag;
            }
        }
    }
}