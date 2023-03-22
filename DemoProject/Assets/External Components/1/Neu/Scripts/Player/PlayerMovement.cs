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