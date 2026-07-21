using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnUIManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI turnText; // Texto que muestra el turno
    public Image pawnImage;          // Imagen donde se muestra la ficha

    [Header("Sprites de ficha por jugador")]
    public Sprite pawnAzulSprite;
    public Sprite pawnAmarilloSprite;
    public Sprite pawnVerdeSprite;
    public Sprite pawnRojoSprite;

    [Header("Colores de cada jugador")]
    public Color colorJugadorAzul = Color.blue;
    public Color colorJugadorAmarillo = Color.yellow;
    public Color colorJugadorVerde = Color.green;
    public Color colorJugadorRojo = Color.red;

    // Llamado por BoardManager cada vez que cambia el turno
    public void UpdateTurnUI(int playerIndex, string playerName)
    {
        // 1️⃣ Actualizar texto y color
        turnText.text = $"{playerName.ToUpper()}";
        turnText.color = GetPlayerColor(playerIndex);

        // 2️⃣ Actualizar sprite de ficha
        UpdatePawnVisual(playerIndex);
    }

    private void UpdatePawnVisual(int playerIndex)
    {
        if (pawnImage == null)
        {
            Debug.LogWarning("TurnUIManager: pawnImage no asignado");
            return;
        }

        Sprite sprite = GetPawnSprite(playerIndex);
        if (sprite == null)
        {
            Debug.LogWarning($"TurnUIManager: No hay sprite para el jugador {playerIndex}");
            pawnImage.enabled = false;
            return;
        }

        pawnImage.sprite = sprite;
        pawnImage.enabled = true;
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

    public Color GetPlayerColor(int playerIndex)
    {
        switch (playerIndex)
        {
            case 0: return colorJugadorAzul;
            case 1: return colorJugadorAmarillo;
            case 2: return colorJugadorVerde;
            case 3: return colorJugadorRojo;
            default: return Color.white;
        }
    }
}
