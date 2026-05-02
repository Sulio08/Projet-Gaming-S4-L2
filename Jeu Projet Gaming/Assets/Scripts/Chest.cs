using UnityEngine;
using System.Collections;

public class Chest : MonoBehaviour
{
    [Header("Coffre")]
    public string keyId;
    public KeyCode openKey = KeyCode.E;
    public Animator animator;

    private bool _playerNearby = false;
    private bool _isOpened = false;

    private void Update()
    {
        if (_playerNearby && !_isOpened && Input.GetKeyDown(openKey))
        {
            OpenChest();
        }
    }

    private void OpenChest()
    {
        _isOpened = true;

        // Déclenche l'animation une seule fois
        if (animator != null)
            animator.SetTrigger("Open");

        // Donne la clé au joueur
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            KeyInventory inventory = player.GetComponent<KeyInventory>();
            if (inventory != null)
                inventory.AddKey(keyId);
        }

        // Affiche le message via le MessageManager global
        MessageManager.Instance.AfficherMessage("Vous avez récupéré une clé !");
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