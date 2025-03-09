using UnityEngine;
using UnityEngine.InputSystem;

public class MovementInputHandler : InputHandler
{
    [SerializeField] private PlayerController playerController;

    private Vector2 moveInput;

    protected override void RegisterInputActions()
    {
        PlayerInput playerInput = GetPlayerInput();
        if(playerInput != null)
        {
            playerInput.actions["Move"].performed += OnMovePerformed;
            playerInput.actions["Move"].canceled += OnMoveCanceled;
            playerInput.actions["Dash"].started += OnDashStarted;
        }
        else
        {
            Debug.Log("PlayerInput is null in MovementInputHandler");
        }
    }

    protected override void UnregisterInputActions()
    {
        PlayerInput playerInput = GetPlayerInput();
        if (playerInput != null)
        {
            playerInput.actions["Move"].performed -= OnMovePerformed;
            playerInput.actions["Move"].canceled -= OnMoveCanceled;
            playerInput.actions["Dash"].started -= OnDashStarted;
        }
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (playerController != null)
        {
            playerController.SetMoveDirection(moveInput);
        }
        else
        {
            Debug.LogError("PlayerController not assigned in MovementInputHandler");
        }
    }


    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;

        if(playerController != null)
        {
            playerController.SetMoveDirection(moveInput);
        }
    }

    private void OnDashStarted(InputAction.CallbackContext context)
    {
        //moveInput = context.ReadValue<Vector2>();
        if (playerController != null)
        {
            playerController.ExecuteDash();
        }
    }
}
