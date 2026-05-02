using UnityEngine;
using TMPro;
using System.Collections;

public class MessageManager : MonoBehaviour
{
    public static MessageManager Instance; // Accessible depuis n'importe quel script

    [Header("UI")]
    public GameObject messagePanel;
    public TMP_Text messageText;
    public float messageDuration = 2f;

    private Coroutine _currentCoroutine;

    private void Awake()
    {
        // Singleton : une seule instance dans toute la scène
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (messagePanel != null)
            messagePanel.SetActive(false);
    }

    public void AfficherMessage(string message)
    {
        // Si un message est déjà affiché, on l'arrête et on affiche le nouveau
        if (_currentCoroutine != null)
            StopCoroutine(_currentCoroutine);

        _currentCoroutine = StartCoroutine(ShowMessage(message));
    }

    private IEnumerator ShowMessage(string message)
    {
        messageText.text = message;
        messagePanel.SetActive(true);

        yield return new WaitForSeconds(messageDuration);

        messagePanel.SetActive(false);
    }
}