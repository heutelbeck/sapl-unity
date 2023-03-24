/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
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