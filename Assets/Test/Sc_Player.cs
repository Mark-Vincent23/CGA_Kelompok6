using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Player : MonoBehaviour
{
    public float movementSpeed = 7.0f;
    public float jumpForce = 4.0f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public Sc_FreeCam fc;
    
    private bool canJump = true;
    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        MovePlayer();
        Jump();
    }

    void MovePlayer()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 movement = (forward * moveVertical + right * moveHorizontal).normalized;

        if (movement != Vector3.zero && !fc.isShiftlock)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        transform.position += movement * movementSpeed * Time.deltaTime;
    }

    void Jump(){
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);

        if (isGrounded && Input.GetButton("Jump") && canJump){
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            canJump = false;
        }else if (isGrounded){
            canJump = true;
        }
    }
}
