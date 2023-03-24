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
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    [SerializeField] private CharacterController controller;
    [SerializeField] private float speed = 12f;
    [SerializeField] private float rightBoundary = 9f;
    [SerializeField] private float leftBoundary = -9f;
    [SerializeField] private float backBoundary = 9f;
    [SerializeField] private float frontBoundary = -9f;

    void Update() {

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        if (controller.transform.position.x >= rightBoundary) {
            controller.transform.position = new Vector3(rightBoundary, controller.transform.position.y, controller.transform.position.z);
        }
        else if (controller.transform.position.x <= leftBoundary) {
            controller.transform.position = new Vector3(leftBoundary, controller.transform.position.y, controller.transform.position.z);
        }

        if (controller.transform.position.z >= backBoundary) {
            controller.transform.position = new Vector3(controller.transform.position.x, controller.transform.position.y, backBoundary);
        }
        else if (controller.transform.position.z <= frontBoundary) {
            controller.transform.position = new Vector3(controller.transform.position.x, controller.transform.position.y, frontBoundary);
        }

    }
}