using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

public class MousePosition : MonoBehaviour {

    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask layerMask;

    private void Update() {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, layerMask)) {
            transform.position = raycastHit.point;
        }
    }

}