using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuJugadoresControl : MonoBehaviour
{
    public TMP_InputField[] inputJugadores; // Arrastra aquí los 4 InputFields

    void Start()
    {
        // Recupera el número de jugadores desde la escena anterior
        int numeroJugadores = PlayerPrefs.GetInt("JugadorSeleccionado", 1);

        // Activar solo los InputFields correspondientes
        for (int i = 0; i < inputJugadores.Length; i++)
        {
            if (i < numeroJugadores)
            {
                inputJugadores[i].interactable = true;
            }
            else
            {
                inputJugadores[i].interactable = false;
                inputJugadores[i].text = ""; // opcional: limpiar texto
            }
        }
    }

    // Método opcional para recoger nombres de jugadores
    public string[] ObtenerNombres()
    {
        int numeroJugadores = PlayerPrefs.GetInt("JugadorSeleccionado", 1);
        string[] nombres = new string[numeroJugadores];

        for (int i = 0; i < numeroJugadores; i++)
        {
            nombres[i] = inputJugadores[i].text;
        }

        return nombres;
    }

    public void BotonComenzar()
    {
        string[] nombres = ObtenerNombres();
        // Aquí puedes guardarlos en PlayerPrefs o pasarlos a tu GameManager
        for (int i = 0; i < nombres.Length; i++)
        {
            PlayerPrefs.SetString("Jugador" + (i + 1), nombres[i]);
        }

        SceneManager.LoadScene("SampleScene");
    }

}
