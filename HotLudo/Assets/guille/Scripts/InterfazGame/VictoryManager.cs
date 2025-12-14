using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VictoryUIManager : MonoBehaviour
{
    [Header("UI")]
    public Image backgroundPanel;       // Panel de fondo que cambia de color
    public TextMeshProUGUI victoryText; // Texto que dir� "HAS GANADO"
    public Image pawnImage;             // Sprite de la ficha ganadora

    [Header("Sprites de ficha por jugador")]
    public Sprite pawnAzulSprite;
    public Sprite pawnAmarilloSprite;
    public Sprite pawnVerdeSprite;
    public Sprite pawnRojoSprite;

    [Header("Colores de fondo por jugador")]
    public Color colorJugadorAzul = Color.blue;
    public Color colorJugadorAmarillo = Color.yellow;
    public Color colorJugadorVerde = Color.green;
    public Color colorJugadorRojo = Color.red;

    [Header("Audio de victoria")]
    public AudioClip victoryClip;  // Arrastra aquí el clip
    private AudioSource audioSource;

    void Start()
    {
        // Configurar AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Reproducir audio una sola vez
        if (victoryClip != null)
            audioSource.PlayOneShot(victoryClip);

        int winnerIndex = PlayerPrefs.GetInt("WinnerIndex", -1);

        if (winnerIndex < 0 || winnerIndex > 3)
        {
            Debug.LogWarning("VictoryUIManager: WinnerIndex no v�lido");
            if (victoryText != null) victoryText.text = "SIN GANADOR";
            if (pawnImage != null) pawnImage.enabled = false;
            return;
        }

        // Cambiar color del fondo seg�n jugador
        if (backgroundPanel != null)
            backgroundPanel.color = GetPlayerColor(winnerIndex);

        // Texto de victoria en blanco
        if (victoryText != null)
        {
            victoryText.text = "HAS GANADO";
            victoryText.color = Color.white;
        }

        // Mostrar sprite de la ficha ganadora
        if (pawnImage != null)
        {
            pawnImage.sprite = GetPawnSprite(winnerIndex);
            pawnImage.enabled = true;
        }

        // Configurar AudioSource
audioSource = GetComponent<AudioSource>();
if(audioSource == null)
    audioSource = gameObject.AddComponent<AudioSource>();

// Reproducir audio una sola vez
if(victoryClip != null)
    audioSource.PlayOneShot(victoryClip);
    }

    private Sprite GetPawnSprite(int playerIndex)
    {
        switch (playerIndex)
        {
            case 0: return pawnAzulSprite;
            case 1: return pawnAmarilloSprite;
            case 2: return pawnVerdeSprite;
            case 3: return pawnRojoSprite;
            default: return null;
        }
    }

    private Color GetPlayerColor(int playerIndex)
    {
        switch (playerIndex)
        {
            case 0: return colorJugadorAzul;
            case 1: return colorJugadorAmarillo;
            case 2: return colorJugadorVerde;
            case 3: return colorJugadorRojo;
            default: return Color.black;
        }
    }
}
