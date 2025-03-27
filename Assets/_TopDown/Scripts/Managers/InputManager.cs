using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [SerializeField] private PlayerInput playerInput;
    public PlayerInput CurrentPlayerInput
    {
        get
        {
            if (playerInput == null)
            {
                playerInput = FindFirstObjectByType<PlayerInput>();
                if (playerInput == null)
                {
                    Debug.Log("Missing PlayerInput in the scene");
                }
            }
            return playerInput;
        }
    }

    private void Awake()
    {
     if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetPlayerInput(PlayerInput newPlayerInput)
    {
        playerInput = newPlayerInput;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
