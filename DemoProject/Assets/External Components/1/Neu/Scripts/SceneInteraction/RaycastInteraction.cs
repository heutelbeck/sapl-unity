using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Input;

public class RaycastInteraction : MonoBehaviour
{
    [SerializeField] private LayerMask pickUpLayerMask;
    [SerializeField] private float pickUpDistance = 25f;

    private Ray rayFromCamera;
    private RaycastHit raycastHit;
    private GameObject gameObjectHit;
    private GameObject gameObjectParent;

    bool objectFocused;
    PinchSlider scriptPinchSlider;
    ObjectManipulator scriptObjectManipulator;

    void Update() {

        rayFromCamera = gameObject.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(rayFromCamera.origin, rayFromCamera.direction, out raycastHit, pickUpDistance, pickUpLayerMask)) {

            gameObjectHit = raycastHit.transform.gameObject;
            //Debug.Log(gameObjectHit.name);

            if (Input.GetMouseButtonDown(0)) {

                if (gameObjectHit.GetComponent<PressableButtonHoloLens2>() != null) {
                    gameObjectHit.GetComponent<PressableButtonHoloLens2>().ButtonPressed.Invoke();
                }
                return;

            } else {

                if (gameObjectHit.GetComponent<ObjectManipulator>() != null) {
                    if (objectFocused == false) {
                        scriptObjectManipulator = gameObjectHit.GetComponent<ObjectManipulator>();
                        scriptObjectManipulator.OnHoverEntered.Invoke(null);
                        objectFocused = true;
                    }
                    return;
                }

                if (gameObjectHit.GetComponent<NearInteractionGrabbable>() != null) {
                    gameObjectParent = GetParentGameObjectWithScript<PinchSlider>(gameObjectHit);
                    if (gameObjectParent != null) {
                        scriptPinchSlider = gameObjectParent.GetComponent<PinchSlider>();
                        scriptPinchSlider.OnHoverEntered.Invoke(null);
                        objectFocused = true;
                    }
                    return;
                }

            }

            if (objectFocused == true && scriptObjectManipulator != null) {
                scriptObjectManipulator.OnHoverExited.Invoke(null);
                scriptObjectManipulator = null;
                objectFocused = false;
            }
            if (objectFocused == true && scriptPinchSlider != null) {
                scriptPinchSlider.OnHoverExited.Invoke(null);
                scriptPinchSlider = null;
                objectFocused = false;
            }
        }
    }

    public GameObject GetParentGameObjectWithScript<T>(GameObject childGameObject) {
        GameObject gameObjectParent = childGameObject.transform.parent.gameObject;
        while (gameObjectParent.transform.parent != null && gameObjectParent.GetComponent<T>() == null) {
            gameObjectParent = gameObjectParent.transform.parent.gameObject;
        }
        return gameObjectParent.GetComponent<T>() != null ? gameObjectParent : null;
    }

}