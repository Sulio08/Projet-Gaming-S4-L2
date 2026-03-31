using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Paramètres de déplacement")]
    [SerializeField] private float normalSpeed = 5f;
    [SerializeField] private float sprintSpeed = 9f;
    [SerializeField] private float jumpingPower = 12f;

    [Header("Détection du sol")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float checkRadius = 0.2f;

    private Rigidbody2D _rb;
    private float _horizontalMovement;
    private float _currentSpeed;
    private bool _isJumping;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _currentSpeed = normalSpeed;
    }

    // Appelé par l'Input System (Action de type Value / Vector2)
    public void Move(InputAction.CallbackContext context)
    {
        // Lit -1, 1 ou 0 automatiquement. 
        // Quand on relâche, context.ReadValue renvoie 0, ce qui arrête le perso.
        _horizontalMovement = context.ReadValue<Vector2>().x;
    }

    // Appelé par l'Input System (Action de type Button)
    public void Jump(InputAction.CallbackContext context)
    {
        // On vérifie le bouton, le sol, ET qu'on n'est pas déjà en train de sauter
        if (context.performed && IsGrounded() && !_isJumping)
        {
            // On remet la vélocité Y à zéro avant d'appliquer la force pour un saut net
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0f);
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpingPower);
            _isJumping = true;
        }
    }

    // Appelé par l'Input System (Action de type Button)
    public void Sprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _currentSpeed = sprintSpeed;
        }
        else if (context.canceled)
        {
            _currentSpeed = normalSpeed;
        }
    }

    private void Update()
    {
        // Application du mouvement horizontal
        _rb.linearVelocity = new Vector2(_horizontalMovement * _currentSpeed, _rb.linearVelocity.y);

        // Gestion du flag de saut pour l'animation (optionnel)
        if (IsGrounded() && _rb.linearVelocity.y <= 0)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, -2f);
        }
    }

    private bool IsGrounded()
    {
        // Création d'un petit cercle invisible aux pieds du joueur.
        // S'il touche un objet qui est dans la Layer "Ground", renvoie true.
        return Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
    }

    // Permet de voir le cercle de détection dans l'éditeur pour bien le placer
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
    private void FixedUpdate()
    {
        // Déplacement horizontal
        _rb.velocity = new Vector2(_horizontalMovement * _currentSpeed, _rb.velocity.y);

        // Vérifie si au sol
        bool grounded = IsGrounded();

        // Plaque le perso au sol pour suivre les bosses
        if (grounded && _rb.velocity.y <= 0f)
        {
            // Petite force vers le bas
            _rb.velocity = new Vector2(_rb.velocity.x, -2f);
        }

        // Réinitialise le flag de saut
        if (grounded && _rb.velocity.y <= 0.1f)
        {
            _isJumping = false;
        }
    }
}