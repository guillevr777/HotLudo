using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Gestiona el menú de nombres de los jugadores antes de iniciar la partida
/// Permite habilitar solo los inputs necesarios según el número de jugadores seleccionado
/// </summary>
public class MenuJugadoresManager : MonoBehaviour
{
    [Header("Inputs de nombres (ordenados 1 → 4)")]
    public TMP_InputField[] inputJugadores;

    private int numeroJugadores;

    /// <summary>
    /// Obtiene el número de jugadores desde PlayerPrefs y activa los inputs correspondientes
    /// </summary>
    void Start()
    {
        numeroJugadores = PlayerPrefs.GetInt("JugadorSeleccionado", 1);
        ActivarInputs();
    }

    /// <summary>
    /// Activa solo los inputs necesarios según el número de jugadores
    /// Los inputs restantes se desactivan
    /// </summary>
    void ActivarInputs()
    {
        for (int i = 0; i < inputJugadores.Length; i++)
        {
            if (i < numeroJugadores)
            {
                inputJugadores[i].interactable = true;
                inputJugadores[i].gameObject.SetActive(true);
            }
            else
            {
                inputJugadores[i].interactable = false;
                inputJugadores[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Método llamado al presionar el botón "Continuar"
    /// Guarda los nombres de los jugadores y carga la escena principal del juego
    /// </summary>
    public void ContinuarJuego()
    {
        for (int i = 0; i < numeroJugadores; i++)
        {
            string nombre = inputJugadores[i].text;

            if (string.IsNullOrEmpty(nombre))
                nombre = $"Jugador {i + 1}";

            PlayerPrefs.SetString("JugadorNombre_" + i, nombre);
        }

        PlayerPrefs.SetInt("NumeroJugadores", numeroJugadores);
        PlayerPrefs.Save();

        // Cargar la escena del juego
        SceneManager.LoadScene("SampleScene");
    }
}
