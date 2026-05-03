using UnityEngine;
using System.Collections;

public class Chest : MonoBehaviour
{
    public enum ChestType { Cle, Vie }

    [Header("Coffre")]
    public ChestType type = ChestType.Cle;
    public KeyCode openKey = KeyCode.E;
    public Animator animator;

    [Header("Si type = Clé")]
    public string keyId;

    [Header("Si type = Vie")]
    public int healAmount = 1; // Nombre de coeurs récupérés

    private bool _playerNearby = false;
    private bool _isOpened = false;

    private void Update()
    {
        if (_playerNearby && !_isOpened && Input.GetKeyDown(openKey))
            OpenChest();
    }

    private void OpenChest()
    {
        _isOpened = true;

        if (animator != null)
            animator.SetTrigger("Open");

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null) return;

        if (type == ChestType.Cle)
        {
            KeyInventory inventory = playerObj.GetComponent<KeyInventory>();
            if (inventory != null)
                inventory.AddKey(keyId);

            MessageManager.Instance.AfficherMessage("Vous avez récupéré une clé !");
        }
        else if (type == ChestType.Vie)
        {
            PlayerMovement player = playerObj.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.currentHealth = Mathf.Min(player.currentHealth + healAmount, player.maxHealth);
                MessageManager.Instance.AfficherMessage("Vous avez récupéré " + healAmount + " coeur(s) !");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            _playerNearby = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            _playerNearby = false;
    }
}