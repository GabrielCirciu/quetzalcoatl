using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLocomotion : MonoBehaviour {

    CharacterController charController;
    Animator charAnimator;
    Vector3 charDirection, charFinalDirection, gravityVector;
    Transform camTransform;
    int motionHash, speedHash;
    float xInputAxis, zInputAxis, charSpeed, gravity, groundDistance, jumpHeight;
    float targetAngle, finalAngle, turnSmoothTime, turnSmoothVelocity;
    public Transform lookAtObj, groundCheck;
    public LayerMask groundMask;
    bool isGrounded;

    void Start() {
        charController = GetComponent<CharacterController>();
        charAnimator = GetComponent<Animator>();
        motionHash = Animator.StringToHash("Motion");
        speedHash = Animator.StringToHash("Speed");
        camTransform = Camera.main.transform;
        turnSmoothTime = 0.1f;
        gravity = -30f;
        groundDistance = 0.1f;
        jumpHeight = 2f;
    }

    void Update() {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if ( isGrounded && gravityVector.y < 0 ) gravityVector.y = -3f;
        
        xInputAxis = Input.GetAxisRaw("Horizontal");
        zInputAxis = Input.GetAxisRaw("Vertical");
        charDirection = new Vector3(xInputAxis, 0f, zInputAxis).normalized;

        if ( Input.GetKeyDown(KeyCode.Space) )
            if ( charAnimator.GetFloat(motionHash) < 0 ) {
                    charController.height = 1.8f;
                    charController.center = new Vector3(0f, 0.92f, 0f);
                    charAnimator.SetFloat(motionHash, 0f);
                }
                else if ( isGrounded ) {
                    gravityVector.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                    charAnimator.Play("Jump");
                }

        gravityVector.y += Time.deltaTime * gravity;
        charController.Move(gravityVector * Time.deltaTime);

        if ( Input.GetKeyDown(KeyCode.LeftControl) )
            if ( charAnimator.GetFloat(motionHash) < 0 ) {
                    charController.height = 1.8f;
                    charController.center = new Vector3(0f, 0.92f, 0f);
                    charAnimator.SetFloat(motionHash, 0f);
                }
                else {
                    charController.height = 1.5f;
                    charController.center = new Vector3(0f, 0.79f, 0f);
                    charAnimator.SetFloat(motionHash, -1f);
                }

        if ( charDirection.magnitude > 0.1f ) {
            if ( charAnimator.GetFloat(motionHash) < 0 ) { charSpeed = 2f; }
            else if ( Input.GetKey(KeyCode.LeftShift) ) { charSpeed = 10f; }
            else if ( Input.GetKey(KeyCode.LeftAlt) ) { charSpeed = 2f; }
            else { charSpeed = 5f; }

            targetAngle = Mathf.Atan2(charDirection.x, charDirection.z) * Mathf.Rad2Deg + camTransform.eulerAngles.y;
            finalAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, finalAngle, 0f);
            charFinalDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            charController.Move(charFinalDirection * charSpeed * Time.deltaTime);
        }
        else charSpeed = 0f;

        charAnimator.SetFloat(speedHash, charSpeed, 0.1f, Time.deltaTime);
    }

    void OnAnimatorIK() {
        charAnimator.SetLookAtWeight(0.5f);
        charAnimator.SetLookAtPosition(lookAtObj.position);        
    }
}