using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuJugadoresManager : MonoBehaviour
{
    [Header("Inputs de nombres (ordenados 1 → 4)")]
    public TMP_InputField[] inputJugadores;

    private int numeroJugadores;

    void Start()
    {
        numeroJugadores = PlayerPrefs.GetInt("JugadorSeleccionado", 1);

        ActivarInputs();
    }

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

        SceneManager.LoadScene("SampleScene");
    }
}
