using UnityEngine;
using UnityEngine.InputSystem;

public abstract class InputHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        if (InputManager.Instance != null && InputManager.Instance.CurrentPlayerInput != null)
        {
            RegisterInputActions();
        }
        else
        {
            Debug.Log($"InputHandler in {gameObject.name} can't be initializied. InputManager not found or PlayerInut null ");
        }
    }


    protected virtual void OnEnable()
    {
        if(InputManager.Instance != null && InputManager.Instance.CurrentPlayerInput != null)
        {
            UnregisterInputActions();
        }
    }

    protected abstract void RegisterInputActions();

    protected abstract void UnregisterInputActions();

    protected PlayerInput GetPlayerInput()
    {
        if(InputManager.Instance != null)
        {
            return InputManager.Instance.CurrentPlayerInput;
        }
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
