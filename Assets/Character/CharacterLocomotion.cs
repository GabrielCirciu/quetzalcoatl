using UnityEngine;

public class CharacterLocomotion : MonoBehaviour {

    private CharacterController _characterController;
    private Animator _charAnimator;
    private Vector3 _charDirection, _charFinalDirection, _gravityVector;
    private Transform _camTransform;
    private int _motionHash, _speedHash;
    private float _xInputAxis, _zInputAxis, _charSpeed, _gravity, _groundDistance, _jumpHeight;
    private float _targetAngle, _finalAngle, _turnSmoothTime, _turnSmoothVelocity;
    public Transform lookAtObj, groundCheck;
    public LayerMask groundMask;
    private bool _isGrounded;

    private void Start() {
        _characterController = GetComponent<CharacterController>();
        _charAnimator = GetComponent<Animator>();
        _motionHash = Animator.StringToHash("Motion");
        _speedHash = Animator.StringToHash("Speed");
        _camTransform = Camera.main.transform;
        _turnSmoothTime = 0.1f;
        _gravity = -30f;
        _groundDistance = 0.3f;
        _jumpHeight = 2f;
    }

    private void Update() {
        _isGrounded = Physics.CheckSphere(groundCheck.position, _groundDistance, groundMask);
        if ( _isGrounded && _gravityVector.y < 0 ) _gravityVector.y = -3f;
        
        _xInputAxis = Input.GetAxisRaw("Horizontal");
        _zInputAxis = Input.GetAxisRaw("Vertical");
        _charDirection = new Vector3(_xInputAxis, 0f, _zInputAxis).normalized;

        if ( Input.GetKeyDown(KeyCode.Space) )
            if ( _charAnimator.GetFloat(_motionHash) < 0 ) {
                _characterController.height = 1.8f;
                _characterController.center = new Vector3(0f, 0.92f, 0f);
                _charAnimator.SetFloat(_motionHash, 0f);
            }
            else if ( _isGrounded ) {
                _gravityVector.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
                _charAnimator.Play("Jump");
            }

        _gravityVector.y += Time.deltaTime * _gravity;
        _characterController.Move(_gravityVector * Time.deltaTime);

        if ( Input.GetKeyDown(KeyCode.LeftControl) )
            if ( _charAnimator.GetFloat(_motionHash) < 0 ) {
                    _characterController.height = 1.8f;
                    _characterController.center = new Vector3(0f, 0.92f, 0f);
                    _charAnimator.SetFloat(_motionHash, 0f);
            }
            else {
                _characterController.height = 1.5f;
                _characterController.center = new Vector3(0f, 0.79f, 0f);
                _charAnimator.SetFloat(_motionHash, -1f);
            }

        if ( _charDirection.magnitude > 0.1f ) {
            if ( _charAnimator.GetFloat(_motionHash) < 0 ) { _charSpeed = 2f; }
            else if ( Input.GetKey(KeyCode.LeftShift) ) { _charSpeed = 10f; }
            else if ( Input.GetKey(KeyCode.LeftAlt) ) { _charSpeed = 2f; }
            else { _charSpeed = 5f; }

            _targetAngle = Mathf.Atan2(_charDirection.x, _charDirection.z) * Mathf.Rad2Deg + _camTransform.eulerAngles.y;
            _finalAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetAngle, ref _turnSmoothVelocity, _turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, _finalAngle, 0f);
            _charFinalDirection = Quaternion.Euler(0f, _targetAngle, 0f) * Vector3.forward ;
            _characterController.Move(_charFinalDirection * _charSpeed * Time.deltaTime);
        }
        else _charSpeed = 0f;

        _charAnimator.SetFloat(_speedHash, _charSpeed, 0.1f, Time.deltaTime);
    }

    private  void OnAnimatorIK(int playerIndex) {
        _charAnimator.SetLookAtWeight(0.5f);
        _charAnimator.SetLookAtPosition(lookAtObj.position);        
    }
}