using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSwap : MonoBehaviour
{
    private const float NormalTimeScale = 1;
    private const float SwapTimeScale = 0.1f;
    private const float NormalFixedScale = 0.02f;
    private const float SwapFixedScale = NormalFixedScale * SwapTimeScale;

    private InputActionMap _inputMap;

    private void Start()
    {
        _inputMap = GameObject.Find("InputHandler").GetComponent<PlayerInput>().actions.FindActionMap("Player");
        _inputMap["SwapWeapon"].started += OnSwapWeaponStart;
        _inputMap["SwapWeapon"].performed += OnSwapWeaponFinish;
    }

    private void OnSwapWeaponStart(InputAction.CallbackContext context)
    {
        Time.timeScale = SwapTimeScale;
        Time.fixedDeltaTime = SwapFixedScale;
    }

    private void OnSwapWeaponFinish(InputAction.CallbackContext context)
    {
        Time.timeScale = NormalTimeScale;
        Time.fixedDeltaTime = NormalFixedScale;
    }
}
