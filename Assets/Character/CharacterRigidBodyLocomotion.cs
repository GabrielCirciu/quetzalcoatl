using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRigidBodyLocomotion : MonoBehaviour {
    Rigidbody charRigidBody;
    Animator charAnimator;
    Vector3 charDirection, charFinalDirection, gravityVector;
    Transform camTransform;
    int motionHash, speedHash;
    float xInputAxis, zInputAxis, charSpeed, jumpHeight, groundDistance;
    float targetAngle, smoothenedAngle, turnSmoothTime, turnSmoothVelocity;
    public Transform lookAtObj, groundCheck;
    public LayerMask groundMask;
    bool isGrounded, isJumping;

    void Start() {
        charRigidBody = GetComponent<Rigidbody>();
        charAnimator = GetComponent<Animator>();
        motionHash = Animator.StringToHash("Motion");
        speedHash = Animator.StringToHash("Speed");
        camTransform = Camera.main.transform;
        turnSmoothTime = 0.1f;
        groundDistance = 0.1f;
        jumpHeight = 1f;
    }

    void FixedUpdate() {
        MoveCharacter();
    }

    void Update() {
        ReadInputs();
        charAnimator.SetFloat(speedHash, charSpeed, 0.1f, Time.deltaTime);
    }
    
    void ReadInputs() {
        xInputAxis = Input.GetAxisRaw("Horizontal");
        zInputAxis = Input.GetAxisRaw("Vertical");
        charDirection = new Vector3(xInputAxis, 0f, zInputAxis);

        if ( Input.GetKeyDown(KeyCode.Space) ) {
            if ( charAnimator.GetFloat(motionHash) < 0 ) charAnimator.SetFloat(motionHash, 0f);
            else if ( Physics.CheckSphere(groundCheck.position, groundDistance, groundMask) ) isJumping = true;
        }
        if ( Input.GetKeyDown(KeyCode.LeftControl) ) {
            if ( charAnimator.GetFloat(motionHash) < 0 ) charAnimator.SetFloat(motionHash, 0f);
            else charAnimator.SetFloat(motionHash, -1f);
        }
        if ( charDirection.magnitude > 0.5f ) {
            if ( charAnimator.GetFloat(motionHash) < 0 ) { charSpeed = 2f; }
            else if ( Input.GetKey(KeyCode.LeftShift) ) { charSpeed = 10f; }
            else if ( Input.GetKey(KeyCode.LeftAlt) ) { charSpeed = 2f; }
            else { charSpeed = 5f; }

            
        }
        else charSpeed = 0f;
    }

    void MoveCharacter() {
        if ( charDirection.magnitude > 0.1f ) {
            RotateCharacter();
            //charFinalDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward * charSpeed * 0.1f;
            //charRigidBody.AddForce(Vector3.forward * charSpeed * 3f);
            if (charRigidBody.velocity.magnitude < charSpeed*2 )
                charRigidBody.AddRelativeForce(Vector3.forward * 50f);
            //charRigidBody.AddTorque(Vector3.forward * charSpeed * 300f);
            //charFinalDirection = transform.position + Vector3.forward * charSpeed * 0.1f;
            //charRigidBody.MovePosition(charFinalDirection);
            Debug.Log(charRigidBody.velocity);
        }
        if ( isJumping ) JumpCharacter();
    }

    void RotateCharacter() {
        charDirection.Normalize();
        targetAngle = Mathf.Atan2(charDirection.x, charDirection.z) * Mathf.Rad2Deg + camTransform.eulerAngles.y;
        smoothenedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        charRigidBody.MoveRotation(Quaternion.Euler(0f, smoothenedAngle, 0f));
    }

    void JumpCharacter() {
        charRigidBody.AddForce(Vector3.up * jumpHeight * 500f);
        charAnimator.Play("Jump");
        isJumping = false;   
    }

    void OnAnimatorIK() {
        charAnimator.SetLookAtWeight(0.5f);
        charAnimator.SetLookAtPosition(lookAtObj.position);        
    }
}