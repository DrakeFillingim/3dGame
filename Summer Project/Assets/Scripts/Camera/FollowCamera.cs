using UnityEngine;
using UnityEngine.InputSystem;

public class FollowCamera : MonoBehaviour
{
    private const float MaxDistance = 6;
    private const float CameraRayWidth = .25f;
    private const float MaxLockOnDistance = 20;
    private const float LockOnWidth = .5f;
    private const int MinPitch = -14;
    private const int MaxPitch = 60;

    public event System.Action<Transform> LockCamera;
    public event System.Action UnlockCamera;

    public float sensX;
    public float sensY;
    public int maxFramerate = 144;

    private InputActionMap _inputMap;
    private Transform _owner;
    private Transform _lookAt = null;
    public float _pitch = 0;
    public float _yaw = 180;

    private void Start()
    {
        _inputMap = GameObject.Find("InputHandler").GetComponent<PlayerInput>().actions.FindActionMap("Player");
        _inputMap.Enable();
        _inputMap["MouseMotion"].performed += OnMouseMotion;
        _inputMap["LockOn"].performed += OnLockOn;
        LockCamera += OnLockCamera;
        UnlockCamera += OnUnlockCamera;
        _inputMap["Pause"].performed += _ => UnityEditor.EditorApplication.isPaused = true;

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = maxFramerate;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _owner = GameObject.Find("Player/PlayerModel/LookAt").transform;
        _lookAt = _owner;
    }

    private void LateUpdate()
    {
        UpdateCameraTransform();
    }

    private void OnMouseMotion(InputAction.CallbackContext context)
    {
        Vector2 mouseDelta = context.ReadValue<Vector2>();
        _pitch -= mouseDelta.y * sensY * Time.timeScale;
        _pitch = Mathf.Clamp(_pitch, MinPitch, MaxPitch);
        _yaw += mouseDelta.x * sensX * Time.timeScale;
        _yaw %= 360;
    }

    private void OnLockOn(InputAction.CallbackContext context)
    {
        if (Physics.SphereCast(transform.position, LockOnWidth, transform.forward, out RaycastHit rayInfo, MaxLockOnDistance, ~LayerMask.GetMask("Player", "Ground")))
        {
            if (rayInfo.transform.root == _lookAt)
            {
                UnlockCamera?.Invoke();
                return;
            }
            LockCamera?.Invoke(rayInfo.transform.root);
        }
    }

    private void OnLockCamera(Transform lookAt)
    {
        _lookAt = lookAt;
        _inputMap["MouseMotion"].Disable();
    }

    private void OnUnlockCamera()
    {
        _inputMap["MouseMotion"].Enable();
        _lookAt = _owner;

        Vector3 toOwner = transform.position - _owner.position;
        _pitch = Mathf.Asin(toOwner.y / GetDistance()) * Mathf.Rad2Deg;
        _yaw = Mathf.Atan2(toOwner.x, toOwner.z) * Mathf.Rad2Deg;
    }

    private void UpdateCameraTransform()
    {
        transform.position = _owner.position + GetDirection() * GetDistance();

        Vector3 toLookAt = (_lookAt.position - transform.position).normalized;
        Vector3 rotationAngle = Quaternion.LookRotation(toLookAt).eulerAngles;
        transform.rotation = Quaternion.Euler(rotationAngle);
    }

    private Vector3 GetDirection()
    {
        float theta = _pitch * Mathf.Deg2Rad;
        float phi = _yaw * Mathf.Deg2Rad;

        if (_inputMap["MouseMotion"].enabled)
        {
            return new Vector3(Mathf.Sin(phi) * Mathf.Cos(theta), Mathf.Sin(theta), Mathf.Cos(phi) * Mathf.Cos(theta)).normalized;
        }
        else
        {
            return -(_lookAt.position - _owner.position).normalized;
        }
    }

    private float GetDistance()
    {
        // spherecast hit player and stopped but second condition (&& rayInfo.transform.root != _owner.root) prevented returning rayInfo distance
        // find better solution than layerMask
        Vector3 toCamera = transform.position - _owner.position;
        if (Physics.SphereCast(_owner.position, CameraRayWidth, toCamera, out RaycastHit rayInfo, MaxDistance, ~LayerMask.GetMask("Player")))
        {
            return rayInfo.distance;
        }
        return MaxDistance;
    }
}
