using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : Singleton<PlayerInputManager>
{
    [SerializeField] private PlayerInput input;

    private InputAction move;
    private InputAction lookAround;
    private InputAction interact;
    private InputAction space;
    private InputAction shift;
    private InputAction control;
    private InputAction leftClick;

    protected override void Awake()
    {
        base.Awake();

        input.currentActionMap.Enable();

        move = input.currentActionMap.FindAction("Move");
        lookAround = input.currentActionMap.FindAction("Look");
        interact = input.currentActionMap.FindAction("Interact");
        space = input.currentActionMap.FindAction("Jump");
        shift = input.currentActionMap.FindAction("Sprint");
        control = input.currentActionMap.FindAction("Control");
        leftClick = input.currentActionMap.FindAction("LeftClick");
    }

    private void OnEnable()
    {
        move.started += Move_started;
        move.canceled += Move_canceled;
        lookAround.performed += LookAround_performed;
        interact.started += Interact_started;
        interact.canceled += Interact_canceled;
        space.started += Space_started;
        space.canceled += Space_canceled;
        shift.started += Shift_started;
        shift.canceled += Shift_canceled;
        control.started += Control_started;
        control.canceled += Control_canceled;
        leftClick.started += LeftClick_started;
        leftClick.canceled += LeftClick_canceled;
    }

    private void OnDisable()
    {
        move.Disable();
        lookAround.Disable();
        interact.Disable();
        space.Disable();
        shift.Disable();
        control.Disable();
        leftClick.Disable();
    }

    private void LeftClick_canceled(InputAction.CallbackContext obj)
    {
        PublicEvents.LeftClickEnded?.Invoke();
    }

    private void LeftClick_started(InputAction.CallbackContext obj)
    {
        PublicEvents.LeftClickStarted?.Invoke();
    }

    private void Control_canceled(InputAction.CallbackContext obj)
    {
        PublicEvents.ControlEnded?.Invoke();
    }

    private void Control_started(InputAction.CallbackContext obj)
    {
        PublicEvents.ControlStarted?.Invoke();
    }

    private void Shift_canceled(InputAction.CallbackContext obj)
    {
        PublicEvents.ShiftEnded?.Invoke();
    }

    private void Shift_started(InputAction.CallbackContext obj)
    {
        PublicEvents.ShiftStarted?.Invoke();
    }

    private void Space_canceled(InputAction.CallbackContext obj)
    {
        PublicEvents.SpaceEnded?.Invoke();
    }

    private void Space_started(InputAction.CallbackContext obj)
    {
        PublicEvents.SpaceStarted?.Invoke();
    }

    private void Interact_canceled(InputAction.CallbackContext obj)
    {
        PublicEvents.InteractCancelled?.Invoke();
    }

    private void Interact_started(InputAction.CallbackContext obj)
    {
        PublicEvents.InteractStarted?.Invoke();
    }

    private void LookAround_performed(InputAction.CallbackContext obj)
    {
        PublicEvents.MousePosition?.Invoke(lookAround.ReadValue<Vector2>());
    }

    private void Move_canceled(InputAction.CallbackContext obj)
    {
        PublicEvents.MoveDirectionStopped?.Invoke();
    }

    private void Move_started(InputAction.CallbackContext obj)
    {
        PublicEvents.MoveDirectionStarted?.Invoke(move.ReadValue<Vector2>());
    }
}
