using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGrabbable : MonoBehaviour {

    private Rigidbody objectRigidbody;
    private Transform objectGrabPointTransform;
    private float coordinateZ;

    private void Awake() {
        objectRigidbody = GetComponent<Rigidbody>();
        coordinateZ = gameObject.transform.position.z;
    }

    public void Grab(Transform objectGrabPointTransform) {
        this.objectGrabPointTransform = objectGrabPointTransform;
        objectRigidbody.useGravity = false;
    }

    public void Drop(Vector3 newPosition = default(Vector3)) {
        if (newPosition != default(Vector3)) {
            objectRigidbody.MovePosition(newPosition);
        }
        this.objectGrabPointTransform = null;
        objectRigidbody.useGravity = true;
    }

    private void FixedUpdate() {
        
        if (objectGrabPointTransform != null) {
            float lerpSpeed = 15f;
            Vector3 newPosition = Vector3.Lerp(transform.position, objectGrabPointTransform.position, Time.deltaTime * lerpSpeed);
            newPosition.z = coordinateZ;
            //newPosition.z =  Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
            objectRigidbody.MovePosition(newPosition);
        }
    }

}