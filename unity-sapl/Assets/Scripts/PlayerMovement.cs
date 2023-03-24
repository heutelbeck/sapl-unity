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
