using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Déplacement")]
    public float normalSpeed = 5f;
    public float sprintSpeed = 9f;
    public float jumpingPower = 12f;
    public float dashPower = 10f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float checkRadius = 0.2f;

    [Header("Animations")]
    public Animator animator;

    private Rigidbody2D _rb;
    private float _horizontal;
    private float _currentSpeed;
    private bool _isJumping;
    private bool _isSprinting;
    private Vector3 baseScale;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _currentSpeed = normalSpeed;
        baseScale = transform.localScale;

        if(animator == null)
            animator = GetComponent<Animator>();
    }

    // ---------------- INPUT ----------------
    public void Move(InputAction.CallbackContext context)
    {
        _horizontal = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if(context.performed && IsGrounded() && !_isJumping)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpingPower);
            _isJumping = true;
            animator.SetTrigger("Jump");
        }
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            _isSprinting = true;
            _currentSpeed = sprintSpeed;
        }
        else if(context.canceled)
        {
            _isSprinting = false;
            _currentSpeed = normalSpeed;
        }
        animator.SetBool("IsSprinting", _isSprinting);
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            animator.SetTrigger("Dash");
            Vector2 dashDir = new Vector2(Mathf.Sign(transform.localScale.x), 0);
            _rb.linearVelocity = dashDir * dashPower;
        }
    }

    // ---------------- UPDATE ----------------
    private void Update()
    {
        // Déplacement horizontal
        _rb.linearVelocity = new Vector2(_horizontal * _currentSpeed, _rb.linearVelocity.y);

        // Flip du sprite
        if(_horizontal > 0) transform.localScale = new Vector3(baseScale.x, baseScale.y, baseScale.z);
        else if(_horizontal < 0) transform.localScale = new Vector3(-baseScale.x, baseScale.y, baseScale.z);

        // Animations
        animator.SetFloat("Speed", Mathf.Abs(_rb.linearVelocity.x));
        animator.SetBool("IsGrounded", IsGrounded());

        if(IsGrounded() && _rb.linearVelocity.y <= 0.1f)
            _isJumping = false;
    }

    // ---------------- UTILITAIRES ----------------
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        if(groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
}