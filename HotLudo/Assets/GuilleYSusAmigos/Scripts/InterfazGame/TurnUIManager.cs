using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Gestiona la interfaz de usuario que indica de quién es el turno
/// </summary>
public class TurnUIManager : MonoBehaviour
{
    [Header("UI")]
    // Texto que muestra el turno
    public TextMeshProUGUI turnText; 
    // Imagen donde se muestra la ficha
    public Image pawnImage;          

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

    /// <summary>
    /// Actualiza la UI del turno
    /// </summary>
    /// <param name="playerIndex">Índice del jugador actual</param>
    /// <param name="playerName">Nombre del jugador actual</param>
    public void UpdateTurnUI(int playerIndex, string playerName)
    {
        // Actualizar texto y color
        turnText.text = $"{playerName.ToUpper()}";
        turnText.color = GetPlayerColor(playerIndex);

        // Actualizar sprite de ficha
        UpdatePawnVisual(playerIndex);
    }

    /// <summary>
    /// Actualiza la imagen de la ficha en la UI según el jugador
    /// </summary>
    /// <param name="playerIndex">Índice del jugador actual</param>
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

    /// <summary>
    /// Devuelve el sprite correspondiente al jugador según su índice
    /// </summary>
    /// <param name="playerIndex">Índice del jugador</param>
    /// <returns>Sprite de la ficha del jugador</returns>
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

    /// <summary>
    /// Devuelve el color correspondiente al jugador según su índice
    /// </summary>
    /// <param name="playerIndex">Índice del jugador</param>
    /// <returns>Color del jugador</returns>
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
