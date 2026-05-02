using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attaque")]
    public int damage = 1;
    public float attackRange = 1.2f;
    public LayerMask enemyLayer;

    [Header("Animations")]
    public Animator animator;

    private bool _canAttack = true;
    public float attackCooldown = 0.5f;
    private float _attackTimer;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (_attackTimer > 0)
        {
            _attackTimer -= Time.deltaTime;
            _canAttack = _attackTimer <= 0;
        }
    }

    // Assigne cette méthode à ta touche d'attaque dans le Input Action Asset
    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed && _canAttack)
        {
            _canAttack = false;
            _attackTimer = attackCooldown;

            animator.SetTrigger("Hit");

            // Détecte les ennemis dans la zone d'attaque
            Vector2 attackDir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
            Collider2D[] hits = Physics2D.OverlapCircleAll(
                (Vector2)transform.position + attackDir * attackRange * 0.5f,
                attackRange,
                enemyLayer
            );

            foreach (Collider2D hit in hits)
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                    enemy.TakeDamage(damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Affiche la zone d'attaque dans la Scene view
        Gizmos.color = Color.red;
        Vector2 attackDir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        Gizmos.DrawWireSphere(
            (Vector2)transform.position + attackDir * attackRange * 0.5f,
            attackRange
        );
    }
}