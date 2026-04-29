using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

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

    [Header("Vie & Dégâts")]
    public int maxHealth = 10;
    public float knockbackForce = 8f;
    public float knockbackDuration = 0.4f;

    [Header("Invincibilité")]
    public float invincibilityDuration = 1.5f;
    public float blinkInterval = 0.1f;

    [Header("Respawn")]
    public float respawnDelay = 2f;

    // Privés
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private float _horizontal;
    private float _currentSpeed;
    private bool _isJumping;
    private bool _isSprinting;
    private Vector3 _baseScale;

    private int _currentHealth;
    private bool _isKnockedBack;
    private bool _isInvincible;
    private float _invincibilityTimer;
    private float _blinkTimer;

    private bool _isRespawning;
    private Vector3 _lastSafePosition;

    // ------------------------------------------------
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _currentSpeed = normalSpeed;
        _baseScale = transform.localScale;
        _currentHealth = maxHealth;

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    // ---------------- INPUT ----------------
    public void Move(InputAction.CallbackContext context)
    {
        _horizontal = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        // Autorise le saut même pendant l'invincibilité, bloque seulement pendant knockback/respawn
        if (context.performed && IsGrounded() && !_isJumping && !_isKnockedBack && !_isRespawning)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpingPower);
            _isJumping = true;
            animator.SetTrigger("Jump");
        }
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _isSprinting = true;
            _currentSpeed = sprintSpeed;
        }
        else if (context.canceled)
        {
            _isSprinting = false;
            _currentSpeed = normalSpeed;
        }
        animator.SetBool("IsSprinting", _isSprinting);
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && !_isKnockedBack && !_isRespawning)
        {
            animator.SetTrigger("Dash");
            Vector2 dashDir = new Vector2(Mathf.Sign(transform.localScale.x), 0);
            _rb.linearVelocity = dashDir * dashPower;
        }
    }

    // ---------------- UPDATE ----------------
    private void Update()
    {
        // Toujours gérer l'invincibilité (clignotement)
        GererInvincibilite();

        // Bloque les inputs et le mouvement pendant knockback
        if (_isKnockedBack || _isRespawning)
            return;

        // Déplacement horizontal
        _rb.linearVelocity = new Vector2(_horizontal * _currentSpeed, _rb.linearVelocity.y);

        // Flip du sprite
        if (_horizontal > 0) transform.localScale = new Vector3(_baseScale.x, _baseScale.y, _baseScale.z);
        else if (_horizontal < 0) transform.localScale = new Vector3(-_baseScale.x, _baseScale.y, _baseScale.z);

        // Animations
        animator.SetFloat("Speed", Mathf.Abs(_rb.linearVelocity.x));
        animator.SetBool("IsGrounded", IsGrounded());

        // Reset saut
        if (IsGrounded() && _rb.linearVelocity.y <= 0.1f)
            _isJumping = false;

        // Sauvegarde dernière position sûre au sol
        if (IsGrounded())
            _lastSafePosition = transform.position;
    }

    // ---------------- DÉGÂTS ----------------
    public void TakeDamage(int damage, Vector3 sourcePosition)
    {
        if (_isInvincible || _isRespawning) return;

        _currentHealth -= damage;
        Debug.Log("Vie restante : " + _currentHealth);

        if (_currentHealth <= 0)
        {
            Debug.Log("Le joueur est mort !");
            // mort à gérer plus tard
            return;
        }

        StartCoroutine(RespawnRoutine(sourcePosition));
    }

    private IEnumerator RespawnRoutine(Vector3 sourcePosition)
    {
        // Knockback
        _isKnockedBack = true;
        Vector2 knockbackDir = (transform.position - sourcePosition).normalized;
        _rb.linearVelocity = Vector2.zero;
        _rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);

        // Invincibilité + clignotement
        _isInvincible = true;
        _invincibilityTimer = invincibilityDuration;
        _blinkTimer = blinkInterval;

        // Durée du knockback
        yield return new WaitForSeconds(knockbackDuration);
        _isKnockedBack = false;

        // Attendre avant téléportation (le joueur peut bouger librement)
        _isRespawning = true;
        yield return new WaitForSeconds(respawnDelay);

        // Téléportation
        _rb.linearVelocity = Vector2.zero;
        transform.position = _lastSafePosition;
        _isRespawning = false;
    }

    // ---------------- INVINCIBILITÉ ----------------
    private void GererInvincibilite()
    {
        if (!_isInvincible) return;

        _invincibilityTimer -= Time.deltaTime;
        _blinkTimer -= Time.deltaTime;

        if (_blinkTimer <= 0)
        {
            _spriteRenderer.enabled = !_spriteRenderer.enabled;
            _blinkTimer = blinkInterval;
        }

        if (_invincibilityTimer <= 0)
        {
            _isInvincible = false;
            _spriteRenderer.enabled = true;
        }
    }

    // ---------------- UTILITAIRES ----------------
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
}