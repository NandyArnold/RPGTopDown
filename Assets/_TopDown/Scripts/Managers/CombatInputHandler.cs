using UnityEngine;
using UnityEngine.InputSystem;



public class CombatInputHandler :InputHandler
{
    private PlayerCombatSystem playerCombatSystem;

    private void Awake()
    {
        playerCombatSystem = GetComponent<PlayerCombatSystem>();
        if(playerCombatSystem == null)
        {
            Debug.LogError("PlayerCombatSystem component not found on COmbatInputHandler");
        }
    }


    protected override void RegisterInputActions()
    {
        PlayerInput playerInput = GetPlayerInput();
        if(playerInput != null)
        {
            playerInput.actions["Attack"].started += OnAttack;
        }
        else
        {
            Debug.LogError("PlayerInput is null in CombatInputHandler");
        }
    }



    protected override void UnregisterInputActions()
    {
        PlayerInput playerInput = GetPlayerInput();
        if(playerInput !=null)
        {
            playerInput.actions["Attack"].started -= OnAttack;
        }
    }



    private void OnAttack(InputAction.CallbackContext context)
    {
        if(playerCombatSystem != null)
        {
            playerCombatSystem.Attack();
        }
    }
}
