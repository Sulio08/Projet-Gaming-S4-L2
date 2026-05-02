using UnityEngine;

public class HacheHitbox : MonoBehaviour
{
    public int damage = 1;
    public Enemy enemy;
    public PlayerMovement joueur; // Glisse ton joueur ici dans l'Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger : " + other.gameObject.name);

        if (other.gameObject == joueur.gameObject)
            joueur.TakeDamage(damage, enemy.transform.position);
    }
}