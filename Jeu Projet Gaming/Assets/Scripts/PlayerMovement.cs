using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    float _horizontalMovement;
    float speed = 3f;
    Rigidbody2D _rb;
    public Transform groundCheck;
    public LayerMask groundLayer;
    float jumpingPower = 5f;
    float acceleration = 6f;
    private bool buttonPressed = false;
    private bool isJumping;
    float _currentspeed;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        Debug.Log("Awake : Rigidbody2D initialized");
    }

    public void Move(InputAction.CallbackContext context)
    {
        buttonPressed = context.performed;
        Debug.Log("Move called - buttonPressed: " + buttonPressed);
        if (buttonPressed)
        {
            float moveInput = context.ReadValue<Vector2>().x;
            Debug.Log("Move input x: " + moveInput);
            if (moveInput != 0)
            {
                _horizontalMovement = Mathf.Sign(moveInput);
                Debug.Log("Horizontal movement set to: " + _horizontalMovement);
            }
        }
    }

    private void Update()
    {
        Sprint();
        _rb.linearVelocity = new Vector2(_horizontalMovement * speed * _currentAcceleration, _rb.linearVelocity.y);
    

        bool grounded = IsGrounded();
        
        if (grounded && _rb.linearVelocity.y <= 0)
        {
            isJumping = false;
    
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
       
        if (context.performed)
        {
            if (!isJumping && IsGrounded())
            {
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpingPower);
                isJumping = true;
            }
        }
    }

    private bool IsGrounded()
    {
        if (groundCheck == null)
        {
            Debug.Log("IsGrounded - groundCheck is null");
            return false;
        }
        Collider2D collider = Physics2D.OverlapCircle(groundCheck.position, 0.15f, groundLayer);
        bool grounded = collider != null;
        Debug.Log("IsGrounded - grounded: " + grounded + " | groundCheck position: " + groundCheck.position);
        return grounded;
    }




    private void Sprint(){
        if(Input.GetKey(Key.code.LeftShift))
            {
                speed = acceleration;
            }
        else{
                _currentspeed = speed;
        }
    }
    
}
