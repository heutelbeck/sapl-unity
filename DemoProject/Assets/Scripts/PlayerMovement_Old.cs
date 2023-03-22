using UnityEngine;

public class PlayerMovement_Old : MonoBehaviour {

    [SerializeField] private CharacterController controller;
    [SerializeField] private float speed = 12f;
    [SerializeField] private float rightBoundary = 9f;
    [SerializeField] private float leftBoundary = -9f;
    [SerializeField] private float backBoundary = 9f;
    [SerializeField] private float frontBoundary = -9f;

    [HideInInspector]
    public bool canMove = true;
    public Camera playerCamera;
    public float lookSpeed = 1.5f;
    public float lookXLimit = 45.0f;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    void Update() {

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);
        
        if (controller.transform.position.x >= rightBoundary) {
            controller.transform.position = new Vector3(rightBoundary, controller.transform.position.y, controller.transform.position.z);
        } else if (controller.transform.position.x <= leftBoundary) {
            controller.transform.position = new Vector3(leftBoundary, controller.transform.position.y, controller.transform.position.z);
        }

        if (controller.transform.position.z >= backBoundary) {
            controller.transform.position = new Vector3(controller.transform.position.x, controller.transform.position.y, backBoundary);
        }
        else if (controller.transform.position.z <= frontBoundary) {
            controller.transform.position = new Vector3(controller.transform.position.x, controller.transform.position.y, frontBoundary);
        }


        //Camera Rotating
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

    }
}
