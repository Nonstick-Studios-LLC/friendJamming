using System.Collections;
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
    [HideInInspector] public InputAction secrets;
    private InputAction reel;

    private Coroutine movementUpdates;

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
        secrets = input.currentActionMap.FindAction("Why"); // bro what are we doing
        reel = input.currentActionMap.FindAction("Reel In");
    }

    private void OnEnable()
    {
        move.performed += Move_started;
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
        reel.started += Reel_Started;
        reel.canceled += Reeleased;
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
        secrets.Disable();
        reel.Disable();
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
        if(movementUpdates != null)
        {
            StopCoroutine(movementUpdates);
            movementUpdates = null;
        }
        PublicEvents.MoveDirection?.Invoke(move.ReadValue<Vector2>());
        PublicEvents.MoveDirectionStopped?.Invoke();
    }

    private void Move_started(InputAction.CallbackContext obj)
    {
        if(movementUpdates == null)
        {
            movementUpdates = StartCoroutine(ContinuousMovement());
        }
        PublicEvents.MoveDirection?.Invoke(move.ReadValue<Vector2>());
    }

    private bool reelHeld;

    private void Reel_Started(InputAction.CallbackContext obj)
    {
        reelHeld = true;
        PublicEvents.ReelStarted?.Invoke();
        StartCoroutine(ContinuousReelGaming());
    }

    private void Reeleased(InputAction.CallbackContext obj)
    {
        reelHeld = false;
        PublicEvents.Reeleased?.Invoke((float)obj.duration);
    }

    private IEnumerator ContinuousReelGaming()
    {
        float timeStarted = Time.time;
        while (reelHeld)
        {
            PublicEvents.ReelHeld?.Invoke(Time.time - timeStarted);
            yield return null;
        }
    }

    private IEnumerator ContinuousMovement()
    {
        while(true)
        {
            PublicEvents.MoveDirection?.Invoke(move.ReadValue<Vector2>());
            yield return null;
        }
    }
}
