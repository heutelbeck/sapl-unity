﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Input.Utilities
{
    /// <summary>
    /// Helper class for setting up canvases for use in the MRTK.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Canvas))]
    [AddComponentMenu("Scripts/MRTK/Services/CanvasUtility")]
    public class CanvasUtility : MonoBehaviour, IMixedRealityPointerHandler {
        private bool oldIsTargetPositionLockedOnFocusLock = false;
        public void OnPointerClicked(MixedRealityPointerEventData eventData) { }

        public void OnPointerDown(MixedRealityPointerEventData eventData) {
            oldIsTargetPositionLockedOnFocusLock = eventData.Pointer.IsTargetPositionLockedOnFocusLock;
            if (!(eventData.Pointer is IMixedRealityNearPointer) && eventData.Pointer.Controller.IsRotationAvailable) {
                eventData.Pointer.IsTargetPositionLockedOnFocusLock = false;
            }
        }

        public void OnPointerDragged(MixedRealityPointerEventData eventData) { }

        public void OnPointerUp(MixedRealityPointerEventData eventData) {
            eventData.Pointer.IsTargetPositionLockedOnFocusLock = oldIsTargetPositionLockedOnFocusLock;
        }

        private void Start() {
            VerifyCanvasConfiguration();
        }

        /// <summary>
        /// Verifies and updates MRTK related canvas configuration.
        /// </summary>
        public void VerifyCanvasConfiguration() {
            Canvas canvas = GetComponent<Canvas>();
            Debug.Assert(canvas != null);

            if (canvas.worldCamera == null)
            {
                //Debug.Assert(CoreServices.InputSystem?.FocusProvider?.UIRaycastCamera != null, this);
                canvas.worldCamera = CoreServices.InputSystem?.FocusProvider?.UIRaycastCamera;

                if (EventSystem.current == null) {
                    Debug.LogError("No EventSystem detected. UI events will not be propagated to Unity UI.");
                }
            }
            else {
                Debug.LogError("World Space Canvas should have no camera set to work properly with Mixed Reality Toolkit. At runtime, they'll get their camera set automatically.");
            }
        }
    }

}