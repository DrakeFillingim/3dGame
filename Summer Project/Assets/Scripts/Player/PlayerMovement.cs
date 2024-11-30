using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private const float TurnSmoothTime = 0.1f;

    private InputActionMap _inputMap;

    private Transform _cameraPosition;
    private Rigidbody _rb;
    private float walkSpeed = 5;

    private Vector3 _inputDirection = Vector3.zero;
    private Transform _rotateTo = null;
    private float _rotationAngle = 0;

    private float _turnSmoothVelocity = 0;

    private void Start()
    {
        _inputMap = GameObject.Find("InputHandler").GetComponent<PlayerInput>().actions.FindActionMap("Player");
        _inputMap["Move"].performed += OnMove;

        FollowCamera _playerCamera = GameObject.Find("FollowCamera").GetComponent<FollowCamera>();
        _playerCamera.LockCamera += OnLockCamera;
        _playerCamera.UnlockCamera += OnUnlockCamera;
        _cameraPosition = _playerCamera.transform;

        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
    }

    private void Update()
    {
        RotatePlayer();
    }

    private void FixedUpdate()
    {
        ResetVelocity();
        MovePlayer();
        
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 inputValue = context.ReadValue<Vector2>();
        _inputDirection = new Vector3(inputValue.x, 0, inputValue.y).normalized;
    }

    private void OnLockCamera(Transform lookAt)
    {
        _rotateTo = lookAt;
        Vector3 vectorToLook = (_rotateTo.position - transform.position).normalized;
        _rotationAngle = Mathf.Atan2(vectorToLook.x, vectorToLook.z) * Mathf.Rad2Deg;
    }

    private void OnUnlockCamera()
    {
        _rotateTo = null;
    }

    private void ResetVelocity()
    {
        if (_rb.velocity.magnitude <= 0.01f)
        {
            _rb.velocity = Vector3.zero;
        }
    }

    private void MovePlayer()
    {
        Vector3 direction = Vector3.zero;

        if (_inputDirection != Vector3.zero)
        {
            _rotationAngle = Mathf.Atan2(_inputDirection.x, _inputDirection.z) * Mathf.Rad2Deg + _cameraPosition.eulerAngles.y;
            direction = Quaternion.Euler(0, _rotationAngle, 0) * Vector3.forward;
            if (_rotateTo != null)
            {
                Vector3 vectorToLook = (_rotateTo.position - transform.position).normalized;
                _rotationAngle = Mathf.Atan2(vectorToLook.x, vectorToLook.z) * Mathf.Rad2Deg;
            }
        }
        AddMovementForce(direction);
    }

    private void RotatePlayer()
    {
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _rotationAngle, ref _turnSmoothVelocity, TurnSmoothTime);
        transform.rotation = Quaternion.Euler(0, angle, 0);
    }

    private void AddMovementForce(Vector3 direction)
    {
        Vector3 targetVelocity = (direction * walkSpeed);
        Vector3 velocityDifference = (targetVelocity - _rb.velocity) / Time.fixedDeltaTime;
        float coefficient = (Mathf.Abs(targetVelocity.magnitude) > 0) ? .2f : .1f;
        Vector3 movement = coefficient * Mathf.Sign(velocityDifference.magnitude) * velocityDifference;
        _rb.AddForce(movement, ForceMode.Force);
    }
}
