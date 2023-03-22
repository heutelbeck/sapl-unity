using Sapl.Components;
//using Sapl.Internal.ComponentsNew;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    Rigidbody playerRigidBody;

    [SerializeField]
    float movementSpeed = 5f;

    [SerializeField]
    float jumpForce = 5f;

    [SerializeField]
    Transform groundCheck;

    [SerializeField]
    LayerMask ground;

    [SerializeField]
    public string textToBeShown;

    private float horizontalInput;
    private float verticalInput;

    // Start is called before the first frame update
    void Start()
    {
        playerRigidBody = GetComponent<Rigidbody>();
    }



    // Update is called once per frame
    void Update()
    {
         horizontalInput = Input.GetAxis("Horizontal");
         verticalInput = Input.GetAxis("Vertical");

        if (horizontalInput != 0 || verticalInput != 0)
        {
            Move();
            //var m = gameObject.GetComponent<EventMethodEnforcementCustom>();
            //m.Execute<float,float>(Move,horizontalInput, verticalInput, MovementDenied);
            //m.ExecuteMethod();
        }
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            Jump();
        }


        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    playerRigidBody.velocity = new Vector3(0, 5f, 0);
        //}
        //if (Input.GetKey(KeyCode.UpArrow))
        //{
        //    playerRigidBody.velocity = new Vector3(0, 0, 5f);
        //}
        //if (Input.GetKey(KeyCode.DownArrow))
        //{
        //    playerRigidBody.velocity = new Vector3(0, 0, -5f);
        //}
        //if (Input.GetKey(KeyCode.LeftArrow))
        //{
        //    playerRigidBody.velocity = new Vector3(-5f, 0, 0);
        //}
        //if (Input.GetKey(KeyCode.RightArrow))
        //{
        //    playerRigidBody.velocity = new Vector3(5f, 0, 0);
        //}
    }

    public void MovementDenied()
    {
        Debug.Log("Movement denied");
        
    }

    private void Jump()
    {
        playerRigidBody.velocity = new Vector3(playerRigidBody.velocity.x, jumpForce, playerRigidBody.velocity.z);
    }

    public void Move()
    {
        playerRigidBody.velocity = new Vector3(horizontalInput * movementSpeed, playerRigidBody.velocity.y, verticalInput * movementSpeed);
    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, .1f, ground);
    }
}
