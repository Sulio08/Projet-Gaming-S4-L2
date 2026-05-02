using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Porte")]
    public string requiredKeyId;      // Doit correspondre au keyId du coffre (ex: "cle_1")
    public KeyCode openKey = KeyCode.E;

    [Header("Sprites")]
    public Sprite spriteFermee;       // Sprite porte fermée
    public Sprite spriteOuverte;      // Sprite porte ouverte

    [Header("Collider à désactiver à l'ouverture")]
    public Collider2D doorCollider;

    private bool _playerNearby = false;
    private bool _isOpened = false;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        // Assigne le sprite fermé au départ
        if (spriteFermee != null)
            _spriteRenderer.sprite = spriteFermee;
    }

    private void Update()
    {
        if (_playerNearby && !_isOpened && Input.GetKeyDown(openKey))
        {
            TryOpen();
        }
    }

    private void TryOpen()
    {
        Debug.Log("TryOpen appelé !");
        
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.Log("Joueur non trouvé !");
            return;
        }

        KeyInventory inventory = player.GetComponent<KeyInventory>();
        if (inventory == null)
        {
            Debug.Log("KeyInventory non trouvé !");
            return;
        }

        Debug.Log("Clé requise : " + requiredKeyId);
        Debug.Log("Joueur a la clé : " + inventory.HasKey(requiredKeyId));

        if (inventory.HasKey(requiredKeyId))
        {
            inventory.UseKey(requiredKeyId);
            OpenDoor();
        }
        else
        {
            Debug.Log("Il te faut la clé : " + requiredKeyId);
        }
    }

    private void OpenDoor()
    {
        _isOpened = true;

        // Change le sprite
        if (spriteOuverte != null)
            _spriteRenderer.sprite = spriteOuverte;

        // Désactive le collider pour que le joueur puisse passer
        if (doorCollider != null)
            doorCollider.enabled = false;

        Debug.Log("Porte ouverte !");
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