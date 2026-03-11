using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    float _horizontalMovement;
    float normalSpeed = 3f;
    float sprintSpeed = 6f;
    float _currentSpeed;
    Rigidbody2D _rb;
    public Transform groundCheck;
    public LayerMask groundLayer;
    float jumpingPower = 5f;
   
    private bool buttonPressed = false;
    private bool isJumping;
    

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _currentSpeed = normalSpeed;
    
    }

    public void Move(InputAction.CallbackContext context)
    {
        buttonPressed = context.performed;
        if (buttonPressed)
        {
            float moveInput = context.ReadValue<Vector2>().x;
          
            if (moveInput != 0)
            {
                _horizontalMovement = Mathf.Sign(moveInput);
            }
        }
    }

    private void Update()
    {
        
        _rb.linearVelocity = new Vector2(_horizontalMovement * normalSpeed * _currentSpeed, _rb.linearVelocity.y);
    

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
            return false;
        }
        Collider2D collider = Physics2D.OverlapCircle(groundCheck.position, 0.15f, groundLayer);
        bool grounded = collider != null;
       
        return grounded;
    }




    public void Sprint(InputAction.CallbackContext context){
        if(context.performed)
            {
             _currentSpeed = sprintSpeed;
            }
        if (context.canceled){
            _currentSpeed = normalSpeed;
        }
}
    }
    

