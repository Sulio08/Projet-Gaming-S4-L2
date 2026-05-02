using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("Coeurs")]
    public Image[] coeurs; // Glisse tes 3 images de coeur ici dans l'Inspector

    [Header("Sprites")]
    public Sprite coeurPlein;
    public Sprite coeurVide;

    [Header("Références")]
    public PlayerMovement player;

    void Update()
    {
        MettreAJourCoeurs();
    }

    void MettreAJourCoeurs()
    {
        for (int i = 0; i < coeurs.Length; i++)
        {
            if (i < player.currentHealth)
                coeurs[i].sprite = coeurPlein;
            else
                coeurs[i].sprite = coeurVide;
        }
    }
}