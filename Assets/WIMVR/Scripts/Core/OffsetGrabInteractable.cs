// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


namespace WIMVR.Core {
    public class OffsetGrabInteractable : XRGrabInteractable {
        private Vector3 interactorPosition;
        private Quaternion interactorRotation;


        protected override void OnSelectEnter(XRBaseInteractor interactor) {
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
        }
    }
}