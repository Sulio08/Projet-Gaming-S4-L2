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
    float _currentAcceleration;
    float accelerationPower = 1f;
    private bool buttonPressed = false;
    private bool isJumping;

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
        Debug.Log("Update - velocity before: " + _rb.velocity);
        UpdateAcceleration();
        _rb.velocity = new Vector2(_horizontalMovement * speed * _currentAcceleration, _rb.velocity.y);
        Debug.Log("Update - velocity after: " + _rb.velocity);

        bool grounded = IsGrounded();
        Debug.Log("Update - IsGrounded: " + grounded + " | isJumping: " + isJumping + " | velocity.y: " + _rb.velocity.y);
        if (grounded && _rb.velocity.y <= 0)
        {
            isJumping = false;
            Debug.Log("Update - Player landed, isJumping set to false");
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        Debug.Log("Jump called - context.performed: " + context.performed);
        if (context.performed)
        {
            if (!isJumping && IsGrounded())
            {
                _rb.velocity = new Vector2(_rb.velocity.x, jumpingPower);
                isJumping = true;
                Debug.Log("Jump executed - velocity.y set to: " + jumpingPower + " | isJumping: true");
            }
            else
            {
                Debug.Log("Jump refused - isJumping: " + isJumping + " | IsGrounded: " + IsGrounded());
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

    private void UpdateAcceleration()
    {
        Debug.Log("UpdateAcceleration - currentAcceleration: " + _currentAcceleration + " | buttonPressed: " + buttonPressed);
        if (!buttonPressed && _currentAcceleration > 0)
        {
            if (IsGrounded())
            {
                _currentAcceleration -= accelerationPower * Time.deltaTime;
            }
            else
            {
                _currentAcceleration -= accelerationPower * Time.deltaTime * 0.5f;
            }
            if (_currentAcceleration <= 0f)
            {
                _currentAcceleration = 0f;
                _horizontalMovement = 0f;
                Debug.Log("UpdateAcceleration - Acceleration reached 0, horizontalMovement reset");
            }
        }
        else if (_currentAcceleration < 5)
        {
            _currentAcceleration += accelerationPower * Time.deltaTime;
        }
        Debug.Log("UpdateAcceleration - currentAcceleration after update: " + _currentAcceleration);
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, 0.15f);
        }
    }
}
