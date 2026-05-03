using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Vie")]
    public int maxHealth = 3;
    public bool isBoss = false;
    private int _currentHealth;

    [Header("Patrouille")]
    public float patrolSpeed = 2f;
    public float patrolDistance = 3f;

    [Header("Poursuite")]
    public float chaseSpeed = 4f;
    public float rayLength = 6f;
    public float stopDistance = 1.2f;

    [Header("Attaque")]
    public int damage = 1;
    public float attackCooldown = 1f;
    public GameObject hacheCollider;

    [Header("Animations")]
    public Animator animator;

    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private Transform _player;

    private Vector3 _startPosition;
    private float _attackTimer;
    private bool _isDead = false;

    private enum State { Patrol, Chase }
    private State _state = State.Patrol;
    private float _patrolDir = 1f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _startPosition = transform.position;
        _currentHealth = maxHealth;

        if (animator == null)
            animator = GetComponent<Animator>();

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            _player = playerObj.transform;
    }

    private void Update()
    {
        if (_isDead) return;

        _attackTimer -= Time.deltaTime;

        if (CanSeePlayer())
            _state = State.Chase;
        else
            _state = State.Patrol;

        switch (_state)
        {
            case State.Patrol: Patrol(); break;
            case State.Chase: Chase(); break;
        }

        animator.SetFloat("Speed", Mathf.Abs(_rb.linearVelocity.x));
    }

    private void Patrol()
    {
        _rb.linearVelocity = new Vector2(_patrolDir * patrolSpeed, _rb.linearVelocity.y);
        FlipSprite(_patrolDir);

        if (_patrolDir > 0 && transform.position.x >= _startPosition.x + patrolDistance)
            _patrolDir = -1f;
        else if (_patrolDir < 0 && transform.position.x <= _startPosition.x - patrolDistance)
            _patrolDir = 1f;
    }

    private void Chase()
    {
        if (_player == null) return;

        float distanceJoueur = Vector2.Distance(transform.position, _player.position);

        if (distanceJoueur <= stopDistance)
        {
            _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
            return;
        }

        float dir = _player.position.x > transform.position.x ? 1f : -1f;
        _rb.linearVelocity = new Vector2(dir * chaseSpeed, _rb.linearVelocity.y);
        FlipSprite(dir);
    }

    private bool CanSeePlayer()
    {
        if (_player == null) return false;
        float distance = Vector2.Distance(transform.position, _player.position);
        return distance <= rayLength;
    }

    private void FlipSprite(float dir)
    {
        _spriteRenderer.flipX = dir < 0;
    }

    public void TakeDamage(int damage)
    {
        if (_isDead) return;

        _currentHealth -= damage;
        animator.SetTrigger("Hit");

        if (_currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        _isDead = true;
        _rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("Dead");

        foreach (Collider2D col in GetComponents<Collider2D>())
            col.enabled = false;

        if (isBoss)
            StartCoroutine(MessageBoss());
        else
            Destroy(gameObject, 1.5f);
    }

    private IEnumerator MessageBoss()
    {
        // Attend la fin de l'animation de mort
        yield return new WaitForSeconds(1.5f);
        
        // Affiche le message
        MessageManager.Instance.AfficherMessage("Merci d'avoir joué !");
        
        // Détruit le boss après le message
        yield return new WaitForSeconds(MessageManager.Instance.messageDuration);
        Destroy(gameObject);
    }

    // ---------------- HACHE ----------------
    public void ActiverHache()
    {
        if (hacheCollider != null)
            hacheCollider.SetActive(true);
    }

    public void DesactiverHache()
    {
        if (hacheCollider != null)
            hacheCollider.SetActive(false);
    }

    // ---------------- ATTAQUE ----------------
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (_isDead) return;

        if (collision.gameObject.CompareTag("Player") && _attackTimer <= 0)
        {
            _attackTimer = attackCooldown;
            animator.SetTrigger("Attack");
        }
    }

    // ---------------- GIZMOS ----------------
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rayLength);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}