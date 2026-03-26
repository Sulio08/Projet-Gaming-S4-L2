using UnityEngine;

public class DamageWall : MonoBehaviour
{
    public int health = 10;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            TakeDamage(1);
        }
    }

    void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Debug.Log("Le joueur a 0 de vie");
           
        }
    }
}