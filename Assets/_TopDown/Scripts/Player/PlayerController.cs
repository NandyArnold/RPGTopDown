using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 20f;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    
    private Vector2 moveDirection;
    private bool isFacingRight = true;
    private bool isDashing = false;

    private void Awake()
    {
        if(rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerInput playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            if(InputManager.Instance != null)
            {
                InputManager.Instance.SetPlayerInput(playerInput);
            }
            else
            {
                Debug.Log("InputManager is not in the scene");
            }
        }
        else
        {
            Debug.Log("Missing PlayerInput on GameObject");
        }
    }

    public void SetMoveDirection(Vector2 direction)
    {
        moveDirection = direction;

        if(direction.x > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (direction.x < 0 && isFacingRight)
        {
            Flip();
        }
    }

   

    private void FixedUpdate()
    {
        Move();
        
        if(isDashing)
        {
            Dash();
            isDashing = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void Move()
    {
        if(rb)
        {
            rb.linearVelocity = moveDirection * moveSpeed;
        }
        else
        {
            Debug.Log("RigidBody2D is missing on PlayerController");
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    public void ExecuteDash()
    {
        isDashing = true;
    }
    public void Dash()
    {
        Debug.Log(" player controller????");
        // rb.AddForce(transform.forward * dashSpeed, ForceMode2D.Force);
        // rb.AddForce(transform.up * dashSpeed, ForceMode2D.Impulse);
        // rb.AddForce(transform.right * dashSpeed, ForceMode2D.Impulse);
        // rb.linearVelocity += moveDirection * dashSpeed;

        if(isFacingRight)
        {
            rb.AddForce(transform.right * dashSpeed, ForceMode2D.Impulse);
        }
        else
        {
            rb.AddForce(transform.right * dashSpeed * (-1), ForceMode2D.Impulse);
        }
    }

}

       
          
